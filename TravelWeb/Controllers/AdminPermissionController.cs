using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class AdminPermissionController : Controller //管理員權限設定
    {
        public IActionResult CreateRole() //權限角色管理
        {
            return View();
        }

        public IActionResult EditRole() //權限分配
        {
            return View();
        }

        public IActionResult AssignPermission()
        {
            return View();
        }

        public IActionResult GetPermissionList()  //權限群組
        {
            return View();
        }

        public IActionResult GetRoleList()
        {
            return View();
        }
    }
}
