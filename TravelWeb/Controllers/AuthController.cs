using Microsoft.AspNetCore.Mvc;
using TravelWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace TravelWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly MemberSystemContext _context;

        public AuthController(MemberSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "請輸入帳號與密碼";
                return View();
            }

            string hashedPassword = HashPassword(password);
            MemberList? user = null;

            if (account.Contains("@"))
            {
                user = await _context.MemberLists
                    .FirstOrDefaultAsync(x => x.Email == account
                                           && x.PasswordHash == hashedPassword);
            }
            else
            {
                user = await _context.MemberLists
                    .FirstOrDefaultAsync(x => x.MemberCode == account
                                           && x.PasswordHash == hashedPassword);
            }

            if (user == null)
            {
                ViewBag.Error = "帳號或密碼錯誤";
                return View();
            }

            var info = await _context.MemberInformations
                .FirstOrDefaultAsync(i => i.MemberCode == user.MemberCode);

            if (info != null && info.Status == "停權")
            {
                ViewBag.Error = "您的帳號已被管理員停權，禁止登入！";
                return View();
            }

            if (!user.MemberCode.StartsWith("G"))
            {
                ViewBag.Error = "此為後台管理系統，僅開放管理員登入！";
                return View(); 
            }

            string role = "Admin";

            HttpContext.Session.SetString("UserCode", user.MemberCode);
            HttpContext.Session.SetString("Role", role);

            var loginRecord = new LogInRecord
            {
                MemberCode = user.MemberCode,
                LoginAt = DateTime.Now
            };

            _context.LogInRecords.Add(loginRecord);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "LoginRecords");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }

        }
    }
}
