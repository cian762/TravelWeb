using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelWeb.Models;
using TravelWeb.Filters;


namespace TravelWeb.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemberSystemContext _context;

        public HomeController(ILogger<HomeController> logger, MemberSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userCode = HttpContext.Session.GetString("UserCode");

            if (string.IsNullOrEmpty(userCode))
            {
                ViewBag.IsLoggedIn = false;
                return View();
            }

            ViewBag.IsLoggedIn = true;
            var memberInfo = await _context.MemberInformations.FirstOrDefaultAsync(m => m.MemberCode == userCode);


            var memberAccount = await _context.MemberLists
                .FirstOrDefaultAsync(m => m.MemberCode == userCode);

            string displayName = memberInfo?.Name ?? memberAccount?.Email ?? userCode;

            ViewBag.UserName = displayName;
            ViewBag.AvatarUrl = memberInfo?.AvatarUrl ?? "/images/default-avatar.png";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
