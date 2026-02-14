using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.Attractions.Models; // 步驟 1: 引用你產生的 Model 資料夾

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
            // 從資料庫抓取所有景點，並傳送給 View
            var data = _context.Attractions.ToList();

            return View(data);
        }


        //新增景點
        // 1. 顯示新增頁面 (Get)
        public IActionResult Create()
        {
            return View();
        }

        // 2. 接收表單資料 (Post)
        [HttpPost]
        [ValidateAntiForgeryToken] // 防止跨站請求攻擊的安全機制
        public IActionResult Create(Attraction attraction)
        {
            if (ModelState.IsValid)// 檢查填寫的資料是否符合 Model 規範
            {
                attraction.CreatedAt = DateTime.Now; // 設定建立時間
                _context.Attractions.Add(attraction);// 把資料交給管家
                _context.SaveChanges(); // 叫管家寫入資料庫
                return RedirectToAction(nameof(Index)); // 存完回到列表頁
            }
            return View(attraction);// 如果填錯了，留在原地並顯示錯誤訊息
        }
    }
}
