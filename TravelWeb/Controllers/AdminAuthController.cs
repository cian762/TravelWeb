using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class AdminAuthController : Controller //管理員登入管理
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login() //管理員登入
        {
            return View();
        }

        public IActionResult Logout() //管理員登出
        {
            return View();
        }

        public IActionResult CheckLoginStatus() //Token / Session 驗證
        {
            return View();
        }

    }
}
