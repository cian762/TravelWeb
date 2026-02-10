using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class LoginLogController : Controller //登入紀錄
    {
        public IActionResult GetAdminLoginLog()
        {
            return View();
        }

        public IActionResult GetMemberLoginLog()
        {
            return View();
        }
    }
}
