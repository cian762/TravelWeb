using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Models;

namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class AttractionTicketController : Controller 
    {
        private readonly AttractionsContext _context; 

        public AttractionTicketController(AttractionsContext context)
        {
            _context = context;
        }

        // 只保留這一個 Index 方法
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.AttractionProducts
                .Include(p => p.Attraction)
                .Include(p => p.TicketType)
                  .Where(p => !p.IsDeleted)  // ← 加這行
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }
        // GET: Attractions/AttractionTicket/Create
        public IActionResult Create()
        {
            var attractions = _context.Attractions.ToList();
            ViewBag.AttractionList = new SelectList(attractions, "AttractionId", "Name");
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName");
            return View();
        }

        // POST: Attractions/AttractionTicket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttractionProduct product)
        {
            // 1. 移除不必要的模型驗證，避免導覽屬性為空導致驗證失敗
            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");
            ModelState.Remove("AttractionProductDetail"); // 手動處理詳情，故移除驗證

            if (ModelState.IsValid)
            {
                // 使用事務 (Transaction) 確保兩張表同步寫入成功
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 2. 自動補上建立時間
                        product.CreatedAt = DateTime.Now;

                        // 3. 處理 Status 字串標準化與 IsActive 的連動邏輯
                        product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                        product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                        // 4. 先儲存主表 (AttractionProducts)
                        _context.Add(product);
                        await _context.SaveChangesAsync(); // 此時 product.ProductId 會被自動生成

                        // 5. 處理詳情資料表 (AttractionProductDetails)
                        if (product.AttractionProductDetail != null)
                        {
                            var detail = product.AttractionProductDetail;
                            detail.ProductId = product.ProductId; // 關鍵：綁定剛產生的 ProductId
                            detail.LastUpdatedAt = DateTime.Now;

                            _context.Add(detail);
                            await _context.SaveChangesAsync();
                        }
                        // ↓ 在這裡加入，建立庫存狀態紀錄
                        _context.ProductInventoryStatuses.Add(new ProductInventoryStatus
                        {
                            ProductId = product.ProductId,
                            InventoryMode = "UNLIMITED",
                            SoldQuantity = 0,
                            LastUpdatedAt = DateTime.Now
                        });
                        await _context.SaveChangesAsync();
                        // 提交事務
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = $"票券「{product.Title}」與詳細內容已成功建立！";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        // 發生錯誤，復原資料庫狀態
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "存檔至資料庫時發生錯誤：" + ex.Message);
                    }
                }
            }

            // --- 若失敗，重新準備選單與偵錯訊息 ---
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine("驗證錯誤: " + error);
            }

            ViewBag.AttractionList = new SelectList(_context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            return View(product);
        }

        // --- 狀態切換功能 (IsActive) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, int isActive)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // 1. 設定銷售狀態 (0 或 1)
            product.IsActive = isActive;

            // 2. 【核心連動邏輯】同步更新系統狀態
            if (isActive == 1)
            {
                // 如果手動切換為「上架」，系統狀態強制變更為「正式發布」
                product.Status = "ACTIVE";
            }
            else
            {
                // 如果手動切換為「下架」，系統狀態自動變更為「草稿」
                // 這樣就不會出現「已下架」但系統狀態還在「ACTIVE」的矛盾情況
                product.Status = "DRAFT";
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = isActive == 1
                    ? $"票券「{product.Title}」已連動更新為：正式發布並上架！"
                    : $"票券「{product.Title}」已連動更新為：草稿並下架。";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "切換狀態時發生錯誤：" + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // --- 系統狀態變更 (Status) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            string cleanStatus = status.ToUpper().Trim();
            product.Status = cleanStatus;

            // --- 嚴謹連動邏輯開始 ---
            if (cleanStatus == "ACTIVE")
            {
                // 只有狀態為 ACTIVE 時，銷售狀態才設為 1 (銷售中)
                product.IsActive = 1;
            }
            else
            {
                // 只要不是 ACTIVE (包含 DRAFT, INACTIVE, ARCHIVED)，一律設為 0 (已下架)
                product.IsActive = 0;
            }
            // --- 嚴謹連動邏輯結束 ---

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                string msg = cleanStatus == "ACTIVE" ? "票券已正式發佈並開始銷售！" : "狀態已更新，銷售已停止。";
                TempData["SuccessMessage"] = msg;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "同步失敗：" + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: AttractionTicket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // 抓取產品資料
            var product = await _context.AttractionProducts
          .Include(p => p.AttractionProductDetail)
          .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null) return NotFound();

            // 準備景點下拉選單 (過濾掉被軟刪除的)
            ViewBag.AttractionList = new SelectList(_context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);

            // 準備票種下拉選單
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttractionProduct product, int[] TagIds) // 關鍵：加入 TagIds 參數
        {
            if (id != product.ProductId) return NotFound();

            // 移除導覽屬性驗證，避免 ModelState.IsValid 為 false
            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");
            ModelState.Remove("AttractionProductDetail");
            ModelState.Remove("AttractionProductTags"); // 移除標籤集合驗證

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync(); // 建議使用交易確保資料完整
                try
                {
                    // 1. 標準化狀態與連動銷售狀態
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                    // 2. 更新產品基本資料表
                    _context.Update(product);

                    // 3. 處理詳情資料表 (AttractionProductDetails)
                    var existingDetail = await _context.AttractionProductDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.ProductId == id);

                    if (product.AttractionProductDetail != null)
                    {
                        product.AttractionProductDetail.ProductId = id;
                        product.AttractionProductDetail.LastUpdatedAt = DateTime.Now;

                        if (existingDetail != null)
                            _context.Update(product.AttractionProductDetail);
                        else
                            _context.Add(product.AttractionProductDetail);
                    }

                    // 4. ★ 核心修改：處理標籤 (AttractionProductTags) ★
                    // 先刪除舊的關聯
                    var oldTags = _context.AttractionProductTags.Where(pt => pt.ProductId == id);
                    _context.AttractionProductTags.RemoveRange(oldTags);
                    await _context.SaveChangesAsync(); // 先執行刪除

                    // 插入新的選中關聯
                    if (TagIds != null && TagIds.Length > 0)
                    {
                        foreach (var tagId in TagIds)
                        {
                            _context.AttractionProductTags.Add(new AttractionProductTag
                            {
                                ProductId = id,
                                TagId = tagId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "更新成功！產品資料與標籤已同步存檔。";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    if (!ProductExists(product.ProductId)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "儲存過程中發生錯誤：" + ex.Message);
                }
            }

            // 失敗時重新載入下拉選單與已選標籤
            ViewBag.AttractionList = new SelectList(_context.Attractions, "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes, "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            // 重新載入標籤顯示 (給 Select2 預填)
            ViewBag.CurrentTags = _context.Tags
                .Where(t => TagIds.Contains(t.TagId))
                .Select(t => new SelectListItem { Value = t.TagId.ToString(), Text = t.TagName, Selected = true })
                .ToList();

            return View(product);
        }



        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            // 1. 抓取詳情資料
            var details = await _context.AttractionProductDetails
                                        .FirstOrDefaultAsync(d => d.ProductId == id);

            // 2. 抓取標籤資料 (根據資料庫 image_5c2da5.png 的結構)
            // 這裡直接透過 AttractionProductTags 串接 Tag 抓取名稱
            var tagNames = await _context.AttractionProductTags
                                         .Where(pt => pt.ProductId == id)
                                         .Select(pt => pt.Tag.TagName) // 假設 Tag 表中存名稱的欄位是 TagName
                                         .ToListAsync();

            // 3. 統一回傳內容
            return Json(new
            {
                // 如果 details 是 null，給予預設文字
                contentDetails = details?.ContentDetails ?? "尚無資料",
                usageInstructions = details?.UsageInstructions ?? "尚無資料",
                notes = details?.Notes ?? "無備註",
                // 回傳標籤陣列，若無則傳空陣列 []
                tags = tagNames ?? new List<string>()
            });
        }

        private bool ProductExists(int id)
        {
            return _context.AttractionProducts.Any(e => e.ProductId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // 避免重複收藏（DB 雖有唯一約束，但先在程式層擋掉比較乾淨）
            var exists = await _context.AttractionProductFavorites
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

            if (!exists)
            {
                _context.Add(new AttractionProductFavorite
                {
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var favorite = await _context.AttractionProductFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite != null)
            {
                _context.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            // 軟刪除：只標記，不真的刪除資料
            product.IsDeleted = true;
            product.IsActive = 0;
            product.Status = "ARCHIVED";

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"票券「{product.Title}」已封存刪除。";
            return RedirectToAction(nameof(Index));
        }
    }
   
    }
