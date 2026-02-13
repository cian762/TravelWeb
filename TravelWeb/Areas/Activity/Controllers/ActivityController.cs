using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Activity.Controllers
{
    [Area("Activity")]
    [Route("Act")]
    public class ActivityController : Controller
    {

        //活動總覽
        [HttpGet("Overview")]
        public IActionResult Index()
        {

            return View();
        }

        //活動內容設定
        [HttpGet("Edit")]
        public IActionResult ActivityEdit() 
        {
            return View();
        }

        [HttpPost("ActivityEdit")]
        public IActionResult ActivityEdit(int a) 
        {
            return RedirectToAction(nameof(Index));
        }

        //活動票卷設定
        [HttpGet("Ticket")]
        public IActionResult TicketManage() 
        {
            return View();
        }

        [HttpPost("TicketEdit")]
        public IActionResult TicketManage(int a) 
        {
            return RedirectToAction(nameof(TicketManage));
        }

        // 活動排程設定
        [HttpGet("Schedule")]
        public IActionResult ActivitySetUp() 
        {
            return View();
        }

        [HttpPost("Schedule")]
        public IActionResult ActivitySetUp(int a) 
        {
            return RedirectToAction(nameof(ActivitySetUp));
        }

        //活動熱度分析


    }
}
