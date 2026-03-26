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
            ViewBag.AttractionList = new SelectList(
                _context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name");
            ViewBag.TicketTypeList = new SelectList(
                _context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName");
            ViewBag.AllTags = _context.Tags.OrderBy(t => t.TagId).ToList();
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
            // ── ProductCode 驗證 ──────────────────────────────────
            // 1. 強制 TKT- 開頭
            if (string.IsNullOrWhiteSpace(product.ProductCode) ||
                !product.ProductCode.StartsWith("TKT-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("ProductCode", "產品代碼必須以 TKT- 開頭，例如：TKT-101");
            }
            // 2. TKT- 後面不能是空的
            else if (product.ProductCode.Trim().Length <= 4)
            {
                ModelState.AddModelError("ProductCode", "TKT- 後面請填入代碼編號，例如：TKT-101");
            }
            // 3. 重複檢查（跨全系統唯一）
            else
            {
                var isDuplicate = await _context.AttractionProducts
                    .AnyAsync(p => p.ProductCode == product.ProductCode && !p.IsDeleted);
                if (isDuplicate)
                    ModelState.AddModelError("ProductCode", $"「{product.ProductCode}」已被使用，請換一個代碼");
            }
            // ─────────────────────────────────────────────────────
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    product.CreatedAt = DateTime.Now;
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;

                    // 先把 detail 切開，避免 EF cascade 自動 INSERT
                    var detailToSave = product.AttractionProductDetail;
                    product.AttractionProductDetail = null;

                    _context.Add(product);
                    await _context.SaveChangesAsync(); // 此時 product.ProductId 已生成

                    // 再手動 INSERT detail 一次
                    if (detailToSave != null)
                    {
                        detailToSave.ProductId = product.ProductId;
                        detailToSave.LastUpdatedAt = DateTime.Now;
                        _context.Add(detailToSave);
                        await _context.SaveChangesAsync();
                    }

                    // Chip 選擇只傳數字 id
                    var realTagIds = (TagIds ?? Array.Empty<string>())
                        .Where(t => int.TryParse(t, out _))
                        .Select(int.Parse)
                        .Distinct();

                    foreach (var tagId in realTagIds)
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

                    TempData["SuccessMessage"] = $"票券「{product.Title}」已成功建立！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "存檔至資料庫時發生錯誤：" + ex.Message);
                }
            }

            ViewBag.AttractionList = new SelectList(
                _context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(
                _context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);
            ViewBag.AllTags = _context.Tags.OrderBy(t => t.TagId).ToList();

            return View(product);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.AttractionProducts
                .Include(p => p.AttractionProductDetail)
                .Include(p => p.AttractionProductTags)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.AttractionList = new SelectList(
                _context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(
                _context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);
            ViewBag.AllTags = _context.Tags.OrderBy(t => t.TagId).ToList();
            ViewBag.CurrentTagIds = product.AttractionProductTags
                .Select(pt => pt.TagId).ToList();

            return View(product);
        }

        // POST: Edit
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
            // ── ProductCode 驗證 ──────────────────────────────────
            // 1. 強制 TKT- 開頭
            if (string.IsNullOrWhiteSpace(product.ProductCode) ||
                !product.ProductCode.StartsWith("TKT-", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("ProductCode", "產品代碼必須以 TKT- 開頭，例如：TKT-101");
            }
            // 2. TKT- 後面不能是空的
            else if (product.ProductCode.Trim().Length <= 4)
            {
                ModelState.AddModelError("ProductCode", "TKT- 後面請填入代碼編號，例如：TKT-101");
            }
            // 3. 重複檢查（排除自己這筆）
            else
            {
                var isDuplicate = await _context.AttractionProducts
                    .AnyAsync(p => p.ProductCode == product.ProductCode
                                && p.ProductId != product.ProductId
                                && !p.IsDeleted);
                if (isDuplicate)
                    ModelState.AddModelError("ProductCode", $"「{product.ProductCode}」已被其他票券使用，請換一個代碼");
            }
            // ─────────────────────────────────────────────────────
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";
                    product.IsActive = (product.Status == "ACTIVE") ? 1 : 0;
                    // ↓ 這兩行是新增的
                    var detailToUpdate = product.AttractionProductDetail;
                    product.AttractionProductDetail = null;

                    _context.Update(product);

                    if (detailToUpdate != null)
                    {
                        var existingDetail = await _context.AttractionProductDetails
                            .FirstOrDefaultAsync(d => d.ProductId == id);

                        if (existingDetail != null)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                @"UPDATE [Attractions].[AttractionProductDetails]
                                  SET content_details    = {0},
                                      notes              = {1},
                                      usage_instructions = {2},
                                      includes           = {3},
                                      excludes           = {4},
                                      eligibility        = {5},
                                      cancel_policy      = {6},
                                      validity_note      = {7},
                                      last_updated_at    = {8}
                                  WHERE product_id = {9}",
                           detailToUpdate.ContentDetails,      // ← 改成 detailToUpdate
            detailToUpdate.Notes,
            detailToUpdate.UsageInstructions,
            detailToUpdate.Includes,
            detailToUpdate.Excludes,
            detailToUpdate.Eligibility,
            detailToUpdate.CancelPolicy,
            detailToUpdate.ValidityNote,
            DateTime.Now,
            id);
                        }
                        else
                        {
                            var newDetail = detailToUpdate;         // ← 改成 detailToUpdate
                            newDetail.ProductId = id;
                            newDetail.LastUpdatedAt = DateTime.Now;
                            _context.Add(newDetail);
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Chip 選擇只傳數字 id
                    var realTagIds = (TagIds ?? Array.Empty<string>())
                        .Where(t => int.TryParse(t, out _))
                        .Select(int.Parse)
                        .Distinct();

                    var oldTags = _context.AttractionProductTags.Where(pt => pt.ProductId == id);
                    _context.AttractionProductTags.RemoveRange(oldTags);
                    await _context.SaveChangesAsync();

                    foreach (var tagId in realTagIds)
                    {
                        _context.AttractionProductTags.Add(new AttractionProductTag
                        {
                            ProductId = id,
                            TagId = tagId
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "更新成功！";
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
                _context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);
            ViewBag.TicketTypeList = new SelectList(
                _context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);
            ViewBag.AllTags = _context.Tags.OrderBy(t => t.TagId).ToList();
            ViewBag.CurrentTagIds = (TagIds ?? Array.Empty<string>())
                .Where(t => int.TryParse(t, out _)).Select(int.Parse).ToList();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            // 同時查 product（取 validity_days）和 detail
            var product = await _context.AttractionProducts
                .Where(p => p.ProductId == id)
                .Select(p => new { p.ValidityDays })
                .FirstOrDefaultAsync();

            var details = await _context.AttractionProductDetails
                .OrderByDescending(d => d.LastUpdatedAt)
                .FirstOrDefaultAsync(d => d.ProductId == id);

            var tagNames = await _context.AttractionProductTags
                .Where(pt => pt.ProductId == id)
                .Select(pt => pt.Tag.TagName)
                .ToListAsync();

            return Json(new
            {
                validityDays = product?.ValidityDays,          // ← 新增
                contentDetails = details?.ContentDetails ?? "",
                notes = details?.Notes ?? "無備註",
                usageInstructions = details?.UsageInstructions ?? "",
                includes = details?.Includes ?? "",
                excludes = details?.Excludes ?? "",
                eligibility = details?.Eligibility ?? "",
                cancelPolicy = details?.CancelPolicy ?? "",
                validityNote = details?.ValidityNote ?? "",
                tags = tagNames ?? new List<string>()
            });
        }

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
                    ? $"票券「{product.Title}」已上架！"
                    : $"票券「{product.Title}」已下架。";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "切換狀態時發生錯誤：" + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

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
                TempData["SuccessMessage"] = cleanStatus == "ACTIVE" ? "票券已發佈！" : "狀態已更新。";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "同步失敗：" + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

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
