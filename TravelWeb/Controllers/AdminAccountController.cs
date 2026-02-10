using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class AdminAccountController : Controller//管理員帳號管理
    {
        public IActionResult GetAdminDetail()
        {
            return View();
        }

        public IActionResult CreateAdmin() //管理員帳號建立
        {
            return View();
        }

        public IActionResult EditAdmin() //修改資料
        {
            return View();
        }

        public IActionResult DeleteAdmin() //停權 / 啟用
        {
            return View();
        }

        public IActionResult GetAdminList()
        {
            return View();
        }
    }
}
