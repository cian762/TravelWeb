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

        // 顯示登入頁
        public IActionResult Login()
        {
            return View();
        }

        // 處理登入
        [HttpPost]
        public IActionResult Login(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "請輸入帳號與密碼";
                return View();
            }

            string hashedPassword = HashPassword(password);

            MemberList? user = null;

            // 🔎 判斷是不是 Email
            if (account.Contains("@"))
            {
                user = _context.MemberLists
                    .FirstOrDefault(x => x.Email == account
                                      && x.PasswordHash == hashedPassword);
            }
            else
            {
                user = _context.MemberLists
                    .FirstOrDefault(x => x.MemberCode == account
                                      && x.PasswordHash == hashedPassword);
            }

            if (user == null)
            {
                ViewBag.Error = "帳號或密碼錯誤";
                return View();
            }

            // 🔥 判斷角色（依 MemberCode 開頭）
            string role = "";

            if (user.MemberCode.StartsWith("G"))
            {
                role = "Admin";
            }
            else if (user.MemberCode.StartsWith("M"))
            {
                role = "Member";
            }
            else
            {
                ViewBag.Error = "帳號格式錯誤";
                return View();
            }

            // 存 Session
            HttpContext.Session.SetString("UserCode", user.MemberCode);
            HttpContext.Session.SetString("Role", role);

            // 🔥 登入成功寫入 LoginRecord
            var loginRecord = new LogInRecord
            {
                MemberCode = user.MemberCode,
                LoginAt = DateTime.Now
            };

            _context.LogInRecords.Add(loginRecord);
            _context.SaveChanges();

            // 導向不同頁面
            if (role == "Admin")
            {
                return RedirectToAction("Index", "LoginRecords");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // 密碼雜湊（要跟你註冊時一致）
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
