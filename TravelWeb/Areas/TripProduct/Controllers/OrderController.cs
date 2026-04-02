using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.TripProduct.Services.InterSer;
using TravelWeb.Filters;

namespace TravelWeb.Areas.TripProduct.Controllers
{
    [AdminAuthorize]
    [Area("TripProduct")]
    public class OrderController : Controller
    {

        private readonly IOrder _order;
        public OrderController(IOrder order)
        {
          _order = order;
        }

        public async Task<IActionResult> Index()
        {
            // 1. 抓數字看板 (今日出團、待處理...)
            var summary = await _order.GetOrderBoardSummaryAsync();

            // 2. 抓訂單清單 (最近的訂單)
            var orders = await _order.GetOrderIndexListAsync();

            // 3. 把兩份資料打包送去 View
            // 做法 A: 用 ViewModel 包起來
            // 做法 B: 用 ViewBag 把 summary 送過去，orders 當 Model
            ViewBag.OrderSummary = summary;

            return View("OrderIndex", orders);
        }
    }
}
