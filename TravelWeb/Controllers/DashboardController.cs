using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class DashboardController : Controller //後台首頁
    {
        public IActionResult GetSummary()
        {
            return View();
        }

        public IActionResult GetMemberStatistics()

        {
            return View();

        }

        public IActionResult GetTicketStatistics()
        {
            return View();
        }

        public IActionResult GetSecurityAlert()
        {
            return View();
        }
    }
}
