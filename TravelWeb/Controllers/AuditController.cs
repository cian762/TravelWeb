using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class AuditController : Controller //審核流程
    {
        public IActionResult GetAuditList()
        {
            return View();
        }

        public IActionResult ReviewAudit()
        {
            return View();
        }
    }
}
