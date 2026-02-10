using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class BlacklistController : Controller //黑名單管理
    {
        public IActionResult AddBlacklist() //新增黑名單
        {
            return View();
        }

        public IActionResult RemoveBlacklist() //移除黑名單
        {
            return View();
        }

        public IActionResult GetBlacklist() //黑名單查詢
        {
            return View();
        }
    }
}
