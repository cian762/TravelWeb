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

        [HttpPost]
        public IActionResult Login(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "請輸入帳號與密碼";
                return View();
            }

            string hashedPassword = HashPassword(password);

            // ============================
            // 1️⃣ 先檢查會員 (Member_List)
            // ============================
            MemberList? member = null;

            if (account.Contains("@"))
            {
                member = _context.MemberLists
                    .FirstOrDefault(x => x.Email == account
                                      && x.PasswordHash == hashedPassword);
            }
            else
            {
                member = _context.MemberLists
                    .FirstOrDefault(x => x.MemberCode == account
                                      && x.PasswordHash == hashedPassword);
            }

            if (member != null)
            {
                // 存 Session
                HttpContext.Session.SetString("UserCode", member.MemberCode);
                HttpContext.Session.SetString("Role", "Member");

                // 寫入登入紀錄
                var loginRecord = new LogInRecord
                {
                    MemberCode = member.MemberCode,
                    LoginAt = DateTime.Now
                };

                _context.LogInRecords.Add(loginRecord);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            // ============================
            // 2️⃣ 再檢查管理員 (Administrator)
            // ============================
            Administrator? admin = null;

            if (account.Contains("@"))
            {
                admin = _context.Administrators
                    .FirstOrDefault(x => x.Email == account
                                      && x.PasswordHash == hashedPassword);
            }
            else
            {
                admin = _context.Administrators
                    .FirstOrDefault(x => x.AdminId == account
                                      && x.PasswordHash == hashedPassword);
            }

            if (admin != null)
            {
                HttpContext.Session.SetString("UserCode", admin.AdminId);
                HttpContext.Session.SetString("Role", "Admin");

                return RedirectToAction("Index", "LoginRecords");
            }

            // ============================
            // 都找不到
            // ============================
            ViewBag.Error = "帳號或密碼錯誤";
            return View();
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
