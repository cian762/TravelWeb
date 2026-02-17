using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.Attractions.Models; // 步驟 1: 引用你產生的 Model 資料夾
using Microsoft.AspNetCore.Mvc.Rendering; // 必須引用：處理 SelectList
using Microsoft.EntityFrameworkCore;    // 必須引用：處理 Include 關聯查詢


namespace TravelWeb.Areas.Attractions.Controllers
{

    [Area("Attractions")] // 確保路由正確指向 Attractions 區域
    public class AttractionAssetsController : Controller
    {
        // 步驟 2: 宣告一個私有的「管家」變數（底線 _）
        private readonly AttractionsContext _context;

        // 步驟 3: 【DI 建構子注入】
        // 括號內的 (AttractionsContext context) 就是跟系統說：請把我在 Program.cs 註冊好的那個管家送進來！
        public AttractionAssetsController(AttractionsContext context)
        {
            _context = context; // 把送進來的管家存到 _context，讓下面的 Index 可以用
        }

        public IActionResult Index()
        {
            // 從資料庫抓取所有景點，並傳送給 View使用 Include 抓取關聯的區域資料
            var data = _context.Attractions
                               .Include(a => a.Region) // 假設你的 Model 導覽屬性叫 Region
                               .ToList();
            return View(data);
        }


        //新增景點
        // 1. 顯示新增頁面 (Get)
        public IActionResult Create()
        {
            // 確保這裡對應的是你 Context 裡的 DbSet 名稱
            var regions = _context.TagsRegions
                                    .Where(r => r.Uid == null)
                                    .OrderBy(r => r.RegionId)
                                    .ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");
            return View();





        }

        // 2. 接收表單資料 (Post)
        [HttpPost]
        [ValidateAntiForgeryToken] // 防止跨站請求攻擊的安全機制
        public async Task<IActionResult> Create(Attraction attraction)
        {

         
            if (ModelState.IsValid)// 檢查填寫的資料是否符合 Model 規範
            {
                // A. 設定建立時間
                attraction.CreatedAt = DateTime.Now;

                // B. 設定預設審核狀態：0 代表待審核 (配合你之後要做的審核功能)
                attraction.ApprovalStatus = 0;

                _context.Attractions.Add(attraction);

                // C. 建議使用非同步儲存
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // --- 如果驗證失敗 (ModelState.IsValid == false) ---

            // 修正：必須與 GET 的邏輯一致，否則回傳頁面時下拉選單會壞掉
            var regions = _context.TagsRegions
                .Where(r => r.Uid == null) // 這裡改用我們討論成功的 null 判斷
                .OrderBy(r => r.RegionId)
                .ToList();

            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction); // 帶著剛才填的資料回到頁面顯示錯誤
        }




        // 讓前端 JavaScript 呼叫的 API
        [HttpGet]
        public IActionResult GetSubRegions(int parentId)
        {
            var subRegions = _context.TagsRegions
           .Where(r => r.Uid == parentId)
           .Select(r => new { id = r.RegionId, name = r.RegionName })
           .ToList();

            return Json(subRegions);
        }


        // 實作審核通過功能

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)

        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();
            // 邏輯優化：如果是 0 就變 1，如果是 1 就變 0
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



        //新增一個專門回傳詳細內容的方法。這能讀取你資料表中的所有欄位，包括經緯度、營業時間與交通資訊。
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var attraction = await _context.Attractions
                .Include(a => a.Region) // 包含關聯的區域資料
                .FirstOrDefaultAsync(m => m.AttractionId == id);

            if (attraction == null) return NotFound();

            // 這裡我們直接回傳一個 Partial View 或 JSON
            // 為了簡單，我們先回傳 JSON 給 JavaScript 渲染
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
                region = attraction.Region?.RegionName ?? "未知"
            });
        }

        //刪除按鈕
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

        // GET: 顯示編輯頁面
        public async Task<IActionResult> Edit(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();

            // 【補上這段】載入第一層區域資料，前端 View 的 ddlArea 才有東西可以點
            var regions = _context.TagsRegions
                                    .Where(r => r.Uid == null)
                                    .OrderBy(r => r.RegionId)
                                    .ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction);
        }

        // POST: 執行更新
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attraction attraction)
        {
            if (id != attraction.AttractionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attraction);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "資料更新成功！";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attractions.Any(e => e.AttractionId == id)) return NotFound();
                    else throw;
                }
            }

            // 如果驗證失敗，也要補回第一層選單資料
            var regions = _context.TagsRegions.Where(r => r.Uid == null).OrderBy(r => r.RegionId).ToList();
            ViewBag.RegionList = new SelectList(regions, "RegionId", "RegionName");

            return View(attraction);
        }

    }
}
