using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemberSystemContext _context;

        // 🔥 修正 1：把兩個建構子「合併」成一個！系統才不會崩潰
        public HomeController(ILogger<HomeController> logger, MemberSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 🔥 修正 2：抓取正確的 Session Key (包含 UserCode 或 MemberCode 以防萬一)
            var userCode = HttpContext.Session.GetString("UserCode") ?? HttpContext.Session.GetString("MemberCode");

            // 如果沒登入：告訴畫面顯示「登入狀態 = false」
            if (string.IsNullOrEmpty(userCode))
            {
                ViewBag.IsLoggedIn = false;
                return View();
            }

            // ==========================
            // 如果有登入：抓取資料並顯示
            // ==========================
            ViewBag.IsLoggedIn = true;

            var memberInfo = await _context.MemberInformations
                .FirstOrDefaultAsync(m => m.MemberCode == userCode);

            var memberAccount = await _context.MemberLists
                .FirstOrDefaultAsync(m => m.MemberCode == userCode);

            // 決定要顯示的稱呼 (優先顯示 Name)
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
