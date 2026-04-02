using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; 
using TravelWeb.Models;
using TravelWeb.Filters;


namespace TravelWeb.Controllers
{
    [AdminAuthorize]
    public class AdministratorController : Controller
    {
        private readonly MemberSystemContext _context;

        public AdministratorController(MemberSystemContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "SuperAdmin")
            {
                context.Result = RedirectToAction("Index", "Home");
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetString("AdminId");

            if (string.IsNullOrEmpty(adminId))
                return RedirectToAction("Login", "Auth");

            var admin = _context.Administrators.FirstOrDefault(a => a.AdminId == adminId);

            if (admin == null) return NotFound();

            return View(admin);
        }
    }
}