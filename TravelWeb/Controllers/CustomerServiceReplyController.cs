using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class CustomerServiceReplyController : Controller //客服回覆
    {
        public IActionResult ReplyTicket() //回覆申訴
        {
            return View();
        }

        public IActionResult GetReplyHistory() //留言紀錄
        {
            return View();
        }
    }
}
