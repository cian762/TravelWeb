using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class MemberAccountController : Controller
    {
        private readonly MemberSystemContext _context;
        public MemberAccountController(MemberSystemContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        // ==========================
        // POST: 建立會員帳號
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberList model, string confirmPassword)
        {
            // 1. 排除自動產生的欄位與關聯表
            ModelState.Remove("MemberCode");
            ModelState.Remove("Authorizations");
            ModelState.Remove("ComplaintRecords");
            ModelState.Remove("LogInRecords");
            ModelState.Remove("MemberInformations");

            // 🔥 除錯雷達 1：如果驗證失敗，把隱藏的錯誤原因印在畫面上！
            if (!ModelState.IsValid)
            {
                // 抓出所有沒通過驗證的欄位與原因
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ModelState.AddModelError("", "表單驗證失敗，原因：" + string.Join(" / ", errorMessages));
                return View(model);
            }

            // 2. 基本檢查
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email 不可為空");
                return View(model);
            }

            bool emailExists = await _context.MemberLists.AnyAsync(m => m.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "此 Email 已被註冊");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.PasswordHash))
            {
                ModelState.AddModelError("PasswordHash", "密碼不可為空");
                return View(model);
            }

            if (model.PasswordHash != confirmPassword)
            {
                ModelState.AddModelError("PasswordHash", "兩次輸入的密碼不一致，請重新確認！");
                return View(model);
            }

            // 3. 準備寫入資料庫
            var random = new Random().Next(100, 999);
            model.MemberCode = "M" + DateTime.Now.ToString("yyyyMMddHHmmss") + random;
            model.PasswordHash = HashPassword(model.PasswordHash);

            // 🔥 除錯雷達 2：捕捉「資料庫寫入失敗」的真實原因
            try
            {
                _context.MemberLists.Add(model);
                await _context.SaveChangesAsync();

                // 成功！跳轉到填寫詳細資料頁面
                return RedirectToAction("Create", "MemberInformations", new { id = model.MemberCode });
            }
            catch (Exception ex)
            {
                // 如果 Entity Framework 存檔失敗 (例如欄位長度不符、找不到資料表)
                // 這行會把最深層的真實錯誤訊息抓出來，顯示在網頁上！
                string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "資料庫寫入失敗，真實原因：" + realError);

                return View(model);
            }
        }

        // ==========================
        // 密碼加密方法
        // ==========================
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
