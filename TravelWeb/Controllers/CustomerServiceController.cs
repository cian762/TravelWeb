using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class CustomerServiceController : Controller//申訴客服
    {
        public IActionResult GetTicketList() //申訴單列表
        {
            return View();
        }

        public IActionResult GetTicketDetail()//查看申訴內容
        {
            return View();
        }

        public IActionResult UpdateTicketStatus()//狀態管理
        {
            return View();
        }
    }
}
