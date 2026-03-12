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

        // ==========================================
        // POST: 處理登入 (已升級為非同步，並加入停權攔截)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken] // 建議加上防偽造標籤比較安全
        public async Task<IActionResult> Login(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "請輸入帳號與密碼";
                return View();
            }

            string hashedPassword = HashPassword(password);

            MemberList? user = null;

            // 🔎 1. 判斷是不是 Email，並去資料庫尋找使用者 (改用 await FirstOrDefaultAsync)
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

            // 找不到使用者 -> 報錯
            if (user == null)
            {
                ViewBag.Error = "帳號或密碼錯誤";
                return View();
            }

            // 🚨 2. 【攔截器】檢查帳號是否被停權
            // 去 MemberInformation 表查狀態
            var info = await _context.MemberInformations
                .FirstOrDefaultAsync(i => i.MemberCode == user.MemberCode);

            if (info != null && info.Status == "停權")
            {
                ViewBag.Error = "您的帳號已被管理員停權，禁止登入！";
                return View(); // 踢回登入頁，不給 Session
            }

            // 🔥 3. 判斷角色（依 MemberCode 開頭）
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

            // 4. 存 Session
            HttpContext.Session.SetString("UserCode", user.MemberCode);
            HttpContext.Session.SetString("Role", role);

            // 🔥 改成這樣：只給 MemberCode 和 LoginAt，讓 ID 自動產生！
            var loginRecord = new LogInRecord
            {
                MemberCode = user.MemberCode,
                LoginAt = DateTime.Now
            };

            _context.LogInRecords.Add(loginRecord);
            await _context.SaveChangesAsync();

            // 6. 導向不同頁面
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
