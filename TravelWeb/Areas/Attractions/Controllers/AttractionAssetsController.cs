using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.Attractions.Models; // 步驟 1: 引用你產生的 Model 資料夾

namespace TravelWeb.Areas.Attractions.Controllers
{

    [Area("Attractions")] // 建議補上這行，確保路由正確指向 Attractions 區域
    public class AttractionAssetsController : Controller
    {
        // 步驟 2: 宣告一個私有的「管家」變數（慣例會加底線 _）
        private readonly AttractionsContext _context;

        // 步驟 3: 【DI 建構子注入】
        // 括號內的 (AttractionsContext context) 就是跟系統說：請把我在 Program.cs 註冊好的那個管家送進來！
        public AttractionAssetsController(AttractionsContext context)
        {
            _context = context; // 把送進來的管家存到 _context，讓下面的 Index 可以用
        }

        public IActionResult Index()
        {
            // 測試：從資料庫抓取所有景點，並傳送給 View
            var data = _context.Attractions.ToList();

            return View(data);
        }
    }
}
