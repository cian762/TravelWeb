using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;

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
        public async Task<IActionResult> Edit(int id, AttractionProduct product)
        {
            if (id != product.ProductId) return NotFound();

            // 移除不需由前端傳回的導覽屬性驗證
            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");
            ModelState.Remove("AttractionProductDetail");

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. 標準化狀態與連動銷售狀態
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                    // 2. 更新產品基本資料表 (AttractionProducts)
                    _context.Update(product);

                    // 3. 處理詳情資料表 (AttractionProductDetails)
                    var existingDetail = await _context.AttractionProductDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.ProductId == id);

                    if (product.AttractionProductDetail != null)
                    {
                        // 設定關聯 ID 與更新時間
                        product.AttractionProductDetail.ProductId = id;
                        product.AttractionProductDetail.LastUpdatedAt = DateTime.Now;

                        if (existingDetail != null)
                        {
                            // 情況 A：已有詳情，執行更新 (包含新增的 Notes 欄位)
                            // 注意：EF 的 Update 會根據傳入的物件內容更新所有欄位
                            _context.Update(product.AttractionProductDetail);
                        }
                        else
                        {
                            // 情況 B：還沒有詳情，執行新增
                            _context.Add(product.AttractionProductDetail);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "更新成功！詳情與備註資料已同步存檔。";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "儲存過程中發生錯誤：" + ex.Message);
                }
            }

            ViewBag.AttractionList = new SelectList(_context.Attractions, "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes, "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);
            return View(product);
        }



        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            // 嘗試抓取詳情資料
            var details = await _context.AttractionProductDetails
                                        .FirstOrDefaultAsync(d => d.ProductId == id);

            if (details == null)
            {
                return Json(new
                {
                    contentDetails = "尚無資料",
                    usageInstructions = "尚無資料",
                    notes = "無備註"
                });
            }

            // 回傳包含內容、說明與備註的 JSON
            return Json(new
            {
                contentDetails = details.ContentDetails,
                usageInstructions = details.UsageInstructions,
                notes = details.Notes
            });
        }


        private bool ProductExists(int id)
        {
            return _context.AttractionProducts.Any(e => e.ProductId == id);
        }
    }
   
    }
