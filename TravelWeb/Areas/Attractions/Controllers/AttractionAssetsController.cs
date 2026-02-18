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

        public IActionResult Index()
        {
            var data = _context.Attractions
                               .Include(a => a.Region)
                               .ToList();
            return View(data);
        }

        // 1. 顯示新增頁面 (Get)
        public IActionResult Create()
        {
            var regions = _context.TagsRegions
                                    .Where(r => r.Uid == null)
                                    .OrderBy(r => r.RegionId)
                                    .ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");
            return View();
        }

        // 2. 接收表單資料 (Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 參數加上 List<IFormFile> imageFiles 以接收多圖上傳
        public async Task<IActionResult> Create(Attraction attraction, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                // A. 設定初始欄位
                attraction.CreatedAt = DateTime.Now;
                attraction.ApprovalStatus = 0; // 預設待審核

                // B. 先存景點資料以取得 AttractionId
                _context.Attractions.Add(attraction);
                await _context.SaveChangesAsync();

                // C. 處理圖片上傳
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
                .Include(a => a.Images) // 👈 關鍵：必須包含圖片
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
                // 👈 把所有圖片路徑轉成字串陣列傳給前端
                images = attraction.Images.Select(img => img.ImagePath).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction != null)
            {
                _context.Attractions.Remove(attraction);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "景點已成功刪除！";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var attraction = await _context.Attractions
                                           .Include(a => a.Images)
                                           .FirstOrDefaultAsync(m => m.AttractionId == id);

            if (attraction == null) return NotFound();

            var regions = _context.TagsRegions.Where(r => r.Uid == null).OrderBy(r => r.RegionId).ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attraction attraction, List<IFormFile> imageFiles)
        {
            if (id != attraction.AttractionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attraction);
                    await _context.SaveChangesAsync();

                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        // 👈 關鍵修正：使用 WebRootPath 確保定位到 wwwroot
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

            var regions = _context.TagsRegions.Where(r => r.Uid == null).OrderBy(r => r.RegionId).ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");
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
