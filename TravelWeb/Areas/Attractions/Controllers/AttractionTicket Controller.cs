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

        public async Task<IActionResult> Index(string? keyword, int? isActive)
        {
            var query = _context.AttractionProducts
                .Include(p => p.Attraction)
                .Include(p => p.TicketType)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Attraction.Name.Contains(keyword));

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            ViewBag.Keyword = keyword;
            ViewBag.IsActive = isActive;

            var tickets = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // GET: Create
        public IActionResult Create()
        {
            var attractions = _context.Attractions.Where(a => !a.IsDeleted).ToList();
            ViewBag.AttractionList = new SelectList(attractions, "AttractionId", "Name");
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttractionProduct product, string[] TagIds)
        {
            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");
            ModelState.Remove("AttractionProductDetail");

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    product.CreatedAt = DateTime.Now;
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    if (product.AttractionProductDetail != null)
                    {
                        var detail = product.AttractionProductDetail;
                        detail.ProductId = product.ProductId;
                        detail.LastUpdatedAt = DateTime.Now;
                        _context.Add(detail);
                        await _context.SaveChangesAsync();
                    }

                    var realTagIds = new List<int>();
                    foreach (var tagId in TagIds ?? Array.Empty<string>())
                    {
                        if (tagId.StartsWith("NEW:"))
                        {
                            var tagName = tagId.Substring(4).Trim();
                            if (string.IsNullOrEmpty(tagName)) continue;

                            var existingTag = await _context.Tags
                                .FirstOrDefaultAsync(t => t.TagName == tagName);

                            if (existingTag != null)
                                realTagIds.Add(existingTag.TagId);
                            else
                            {
                                var newTag = new Tag { TagName = tagName };
                                _context.Tags.Add(newTag);
                                await _context.SaveChangesAsync();
                                realTagIds.Add(newTag.TagId);
                            }
                        }
                        else if (int.TryParse(tagId, out int existingId))
                        {
                            realTagIds.Add(existingId);
                        }
                    }

                    foreach (var tagId in realTagIds.Distinct())
                    {
                        _context.AttractionProductTags.Add(new AttractionProductTag
                        {
                            ProductId = product.ProductId,
                            TagId = tagId
                        });
                    }
                    await _context.SaveChangesAsync();

                    _context.ProductInventoryStatuses.Add(new ProductInventoryStatus
                    {
                        ProductId = product.ProductId,
                        InventoryMode = "UNLIMITED",
                        SoldQuantity = 0,
                        LastUpdatedAt = DateTime.Now
                    });
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"票券「{product.Title}」與詳細內容已成功建立！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "存檔至資料庫時發生錯誤：" + ex.Message);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
                System.Diagnostics.Debug.WriteLine("驗證錯誤: " + error);

            ViewBag.AttractionList = new SelectList(_context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            return View(product);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.AttractionProducts
                .Include(p => p.AttractionProductDetail)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.AttractionList = new SelectList(_context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            return View(product);
        }

        // POST: Edit ← 核心修正在這裡
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttractionProduct product, string[] TagIds)
        {
            if (id != product.ProductId) return NotFound();

            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");
            ModelState.Remove("AttractionProductDetail");
            ModelState.Remove("AttractionProductTags");
            ModelState.Remove("Tags");

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                    _context.Update(product);

                    // ── 詳情更新（修正重點）──────────────────────────────
                    if (product.AttractionProductDetail != null)
                    {
                        // 直接用 SQL 查詢，繞過 EF 追蹤問題
                        var existingDetail = await _context.AttractionProductDetails
                            .FromSqlRaw(
                                "SELECT TOP 1 * FROM [Attractions].[AttractionProductDetails] WHERE product_id = {0} ORDER BY last_updated_at DESC",
                                id)
                            .FirstOrDefaultAsync();

                        if (existingDetail != null)
                        {
                            // 有舊資料：直接用 SQL UPDATE，避免 HasNoKey 無法追蹤的問題
                            await _context.Database.ExecuteSqlRawAsync(
                                @"UPDATE [Attractions].[AttractionProductDetails]
                                  SET content_details    = {0},
                                      notes              = {1},
                                      usage_instructions = {2},
                                      includes           = {3},
                                      excludes           = {4},
                                      eligibility        = {5},
                                      cancel_policy      = {6},
                                      last_updated_at    = {7}
                                  WHERE product_id = {8}",
                                product.AttractionProductDetail.ContentDetails,
                                product.AttractionProductDetail.Notes,
                                product.AttractionProductDetail.UsageInstructions,
                                product.AttractionProductDetail.Includes,
                                product.AttractionProductDetail.Excludes,
                                product.AttractionProductDetail.Eligibility,
                                product.AttractionProductDetail.CancelPolicy,
                                DateTime.Now,
                                id);
                        }
                        else
                        {
                            // 第一次新增才 INSERT
                            var newDetail = product.AttractionProductDetail;
                            newDetail.ProductId = id;
                            newDetail.LastUpdatedAt = DateTime.Now;
                            _context.Add(newDetail);
                            await _context.SaveChangesAsync();
                        }
                    }
                    // ────────────────────────────────────────────────────

                    // 標籤處理
                    var realTagIds = new List<int>();
                    foreach (var tagId in TagIds ?? Array.Empty<string>())
                    {
                        if (tagId.StartsWith("NEW:"))
                        {
                            var tagName = tagId.Substring(4).Trim();
                            if (string.IsNullOrEmpty(tagName)) continue;

                            var existingTag = await _context.Tags
                                .FirstOrDefaultAsync(t => t.TagName == tagName);

                            if (existingTag != null)
                                realTagIds.Add(existingTag.TagId);
                            else
                            {
                                var newTag = new Tag { TagName = tagName };
                                _context.Tags.Add(newTag);
                                await _context.SaveChangesAsync();
                                realTagIds.Add(newTag.TagId);
                            }
                        }
                        else if (int.TryParse(tagId, out int existingId))
                        {
                            realTagIds.Add(existingId);
                        }
                    }

                    var oldTags = _context.AttractionProductTags.Where(pt => pt.ProductId == id);
                    _context.AttractionProductTags.RemoveRange(oldTags);
                    await _context.SaveChangesAsync();

                    foreach (var tagId in realTagIds.Distinct())
                    {
                        _context.AttractionProductTags.Add(new AttractionProductTag
                        {
                            ProductId = id,
                            TagId = tagId
                        });
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

            ViewBag.AttractionList = new SelectList(
                _context.Attractions.Where(a => !a.IsDeleted),
                "AttractionId", "Name", product.AttractionId);

            ViewBag.TicketTypeList = new SelectList(
                _context.TicketTypes.OrderBy(t => t.SortOrder),
                "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            var currentTagIds = (TagIds ?? Array.Empty<string>())
                .Where(t => int.TryParse(t, out _))
                .Select(int.Parse)
                .ToList();

            ViewBag.CurrentTags = await _context.Tags
                .Where(t => currentTagIds.Contains(t.TagId))
                .Select(t => new SelectListItem
                {
                    Value = t.TagId.ToString(),
                    Text = t.TagName,
                    Selected = true
                })
                .ToListAsync();

            return View(product);
        }

        // GET: GetDetails（修正：取最新一筆，並回傳新欄位）
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await _context.AttractionProductDetails
                .OrderByDescending(d => d.LastUpdatedAt)  // ← 改為取最新一筆
                .FirstOrDefaultAsync(d => d.ProductId == id);

            var tagNames = await _context.AttractionProductTags
                .Where(pt => pt.ProductId == id)
                .Select(pt => pt.Tag.TagName)
                .ToListAsync();

            return Json(new
            {
                contentDetails = details?.ContentDetails ?? "尚無資料",
                notes = details?.Notes ?? "無備註",
                usageInstructions = details?.UsageInstructions ?? "尚無資料",
                includes = details?.Includes ?? "",
                excludes = details?.Excludes ?? "",
                eligibility = details?.Eligibility ?? "",
                cancelPolicy = details?.CancelPolicy ?? "",
                tags = tagNames ?? new List<string>()
            });
        }

        // 狀態切換
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, int isActive)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = isActive;
            product.Status = isActive == 1 ? "ACTIVE" : "DRAFT";

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

        // 系統狀態變更
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            string cleanStatus = status.ToUpper().Trim();
            product.Status = cleanStatus;
            product.IsActive = cleanStatus == "ACTIVE" ? 1 : 0;

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

        // 軟刪除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            product.IsDeleted = true;
            product.IsActive = 0;
            product.Status = "ARCHIVED";

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"票券「{product.Title}」已封存刪除。";
            return RedirectToAction(nameof(Index));
        }

        // 加入最愛
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

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

        // 移除最愛
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

        private bool ProductExists(int id)
        {
            return _context.AttractionProducts.Any(e => e.ProductId == id);
        }
    }
}
