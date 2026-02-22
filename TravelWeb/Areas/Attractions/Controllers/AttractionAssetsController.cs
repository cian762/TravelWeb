using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using TravelWeb.Areas.Attractions.Models;

namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class AttractionAssetsController : Controller
    {
        private readonly AttractionsContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // 👈 這裡統一名稱為 _hostEnvironment
        public AttractionAssetsController(AttractionsContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment; // 👈 這裡對應賦值
        }

        public async Task<IActionResult> Index(string? keyword, int? approvalStatus)
        {
            var query = _context.Attractions
                                .Include(a => a.Region)
                                .Where(a => !a.IsDeleted)
                                .AsQueryable();

            // 景點名稱關鍵字搜尋
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(a => a.Name.Contains(keyword));

            // 審核狀態篩選 (0=審核中, 1=已核准)
            if (approvalStatus.HasValue)
                query = query.Where(a => a.ApprovalStatus == approvalStatus.Value);

            // 把搜尋條件傳回 View，讓搜尋框保留輸入值
            ViewBag.Keyword = keyword;
            ViewBag.ApprovalStatus = approvalStatus;

            var list = await query.ToListAsync();
            return View(list);
        }

        // 1. 顯示新增頁面 (Get)
        public IActionResult Create()
        {
            // 1. 原有的區域下拉選單
            var regions = _context.TagsRegions
                                    .Where(r => r.Uid == null)
                                    .OrderBy(r => r.RegionId)
                                    .ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            // 2. 新增：抓取所有標籤類別 (對應 AttractionTypeCategories 表)
            // 這樣 View 才能用 foreach 把標籤排出來
            ViewBag.Categories = _context.AttractionTypeCategories.ToList();

            return View();
        }

        // 2. 接收表單資料 (Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 參數加上 List<IFormFile> imageFiles 以接收多圖上傳
        public async Task<IActionResult> Create(Attraction attraction, List<IFormFile> imageFiles, int[] selectedCategoryIds)
        {
            if (ModelState.IsValid)
            {
                // A. 設定初始欄位
                attraction.CreatedAt = DateTime.Now;
                attraction.ApprovalStatus = 0; // 預設待審核

                // B. 先存景點資料以取得 AttractionId
                _context.Attractions.Add(attraction);
                await _context.SaveChangesAsync();

                // 2. 儲存標籤關聯 (Mapping)
                if (selectedCategoryIds != null && selectedCategoryIds.Length > 0)
                {
                    foreach (var typeId in selectedCategoryIds)
                    {
                        var mapping = new AttractionTypeMapping
                        {
                            AttractionId = attraction.AttractionId, // 連結剛產生的景點 ID
                            AttractionTypeId = typeId
                        };
                        _context.AttractionTypeMappings.Add(mapping);
                    }
                    await _context.SaveChangesAsync();
                }

                 // 3. 處理圖片上傳
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    // 設定儲存路徑：wwwroot/uploads/attractions
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "attractions");

                    // 如果資料夾不存在則建立
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // 1. 產生不重複檔名 (GUID)
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(uploadFolder, fileName);

                            // 2. 儲存實體檔案到伺服器
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // 3. 建立 Image 資料庫紀錄
                            var imgRecord = new Image // 你的圖片 Model
                            {
                                AttractionId = attraction.AttractionId, // 關聯剛剛產生的 ID
                                ImagePath = "/uploads/attractions/" + fileName
                            };
                            _context.Images.Add(imgRecord);
                        }
                    }
                    // 儲存所有圖片紀錄
                    await _context.SaveChangesAsync();
                }
                // ✨ [補上這一段]：設定新增成功的提示訊息
                TempData["SuccessMessage"] = $"景點「{attraction.Name}」已成功新增！";
                return RedirectToAction(nameof(Index));
            }

            // --- 若驗證失敗，執行以下邏輯回傳頁面 ---

            // 重新準備區域下拉選單 (否則回傳後下拉選單會變空)
            var regions = _context.TagsRegions
                .Where(r => r.Uid == null)
                .OrderBy(r => r.RegionId)
                .ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction);
        }



        [HttpGet]
        public IActionResult GetSubRegions(int parentId)
        {
            var subRegions = _context.TagsRegions
                           .Where(r => r.Uid == parentId)
                           .Select(r => new { id = r.RegionId, name = r.RegionName })
                           .ToList();
            return Json(subRegions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();

            if (attraction.ApprovalStatus == 0)
            {
                attraction.ApprovalStatus = 1;
                TempData["SuccessMessage"] = $"景點「{attraction.Name}」已通過審核！";
            }
            else
            {
                attraction.ApprovalStatus = 0;
                TempData["SuccessMessage"] = $"景點「{attraction.Name}」狀態已重設為審核中。";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- 重點修改：詳細資訊 API ---
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var attraction = await _context.Attractions
                .Include(a => a.Region)
                .Include(a => a.Images)
                .Include(a => a.AttractionTypeMappings) // 👈 1. 載入中間表
                    .ThenInclude(m => m.AttractionType) // 👈 2. 載入真正的標籤分類名稱
                .FirstOrDefaultAsync(m => m.AttractionId == id);

            if (attraction == null) return NotFound();

            return Json(new
            {
                name = attraction.Name,
                address = attraction.Address,
                phone = attraction.Phone ?? "無",
                website = attraction.Website ?? "無",
                businessHours = attraction.BusinessHours ?? "未設定",
                closedDays = attraction.ClosedDaysNote ?? "無",
                transport = attraction.TransportInfo ?? "無",
                lat = attraction.Latitude,
                lng = attraction.Longitude,
                createdAt = attraction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                region = attraction.Region?.RegionName ?? "未知",
                images = attraction.Images.Select(img => img.ImagePath).ToList(),

                // 👈 3. 把標籤名稱轉成字串陣ate陣列傳給前端
                tags = attraction.AttractionTypeMappings
                                 .Select(m => m.AttractionType.AttractionTypeName)
                                 .ToList()
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();

            // 改為軟刪除：不移除資料，只改狀態
            attraction.IsDeleted = true;

            // 如果希望連動，也可以把該景點下的所有票券也設為下架 (IsActive = 0)
            var relatedProducts = _context.AttractionProducts.Where(p => p.AttractionId == id);
            foreach (var product in relatedProducts)
            {
                product.IsActive = 0;
                product.Status = "INACTIVE";
            }

            _context.Update(attraction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "景點已下架（軟刪除成功）";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var attraction = await _context.Attractions
                                           .Include(a => a.Images)
                                           .FirstOrDefaultAsync(m => m.AttractionId == id);

            if (attraction == null) return NotFound();

            // 💡 重點：編輯頁面也需要抓取所有標籤類別，View 才能跑 foreach
            ViewBag.Categories = _context.AttractionTypeCategories.ToList();

            var regions = _context.TagsRegions.Where(r => r.Uid == null).OrderBy(r => r.RegionId).ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // 💡 補上 selectedCategoryIds 參數
        public async Task<IActionResult> Edit(int id, Attraction attraction, List<IFormFile> imageFiles, int[] selectedCategoryIds)
        {
            if (id != attraction.AttractionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // A. 更新景點主體資料
                    _context.Update(attraction);
                    await _context.SaveChangesAsync();

                    // B. 處理標籤同步 (先清空該景點的所有舊標籤，再新增目前的勾選)
                    // 1. 找出資料庫裡這筆景點現有的所有 Mapping
                    var oldMappings = _context.AttractionTypeMappings
                                             .Where(m => m.AttractionId == id);

                    // 2. 移除它們
                    _context.AttractionTypeMappings.RemoveRange(oldMappings);
                    await _context.SaveChangesAsync();

                    // 3. 重新插入勾選的標籤
                    if (selectedCategoryIds != null && selectedCategoryIds.Length > 0)
                    {
                        foreach (var typeId in selectedCategoryIds)
                        {
                            _context.AttractionTypeMappings.Add(new AttractionTypeMapping
                            {
                                AttractionId = id,
                                AttractionTypeId = typeId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    // C. 處理圖片上傳 (維持你原本的邏輯)
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "attractions");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        foreach (var file in imageFiles)
                        {
                            if (file.Length > 0)
                            {
                                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                string filePath = Path.Combine(uploadDir, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                _context.Images.Add(new Image
                                {
                                    AttractionId = attraction.AttractionId,
                                    ImagePath = "/uploads/attractions/" + fileName
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "更新成功！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "儲存失敗：" + ex.Message);
                }
            }

            // --- 若驗證失敗，重新準備下拉選單與標籤資料 ---
            var regions = _context.TagsRegions.Where(r => r.Uid == null).OrderBy(r => r.RegionId).ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");
            // 💡 別忘了補上標籤清單，不然頁面跳回來標籤會不見
            ViewBag.Categories = _context.AttractionTypeCategories.ToList();

            return View(attraction);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var img = await _context.Images.FirstOrDefaultAsync(i => i.ImageId == id);
            if (img == null) return Json(new { success = false, message = "找不到圖片" });

            try
            {
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }

                _context.Images.Remove(img);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
