using Microsoft.AspNetCore.Mvc;
using TravelWeb.Models;
using System.Security.Cryptography;
using System.Text;
namespace TravelWeb.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly MemberSystemContext _context;

        public AdministratorController(MemberSystemContext context)
        {
            _context = context;
        }

        // 管理員查看自己的資料
        public IActionResult Profile()
        {
            // 確認是否登入
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Auth");
            }

            // 只允許 Admin 或 SuperAdmin
            if (role != "Admin" && role != "SuperAdmin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            // 取得登入者 AdminId
            var adminId = HttpContext.Session.GetString("AdminId");

            if (string.IsNullOrEmpty(adminId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var admin = _context.Administrators
                                .FirstOrDefault(a => a.AdminId == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }
    }
}
