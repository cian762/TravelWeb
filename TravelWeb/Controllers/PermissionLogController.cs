using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class PermissionLogController : Controller //權限執行紀錄
    {
        public IActionResult GetPermissionExecuteLog()
        {
            return View();
        }
    }
}
