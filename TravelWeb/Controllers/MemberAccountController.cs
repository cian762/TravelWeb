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
        public async Task<IActionResult> Create(MemberList model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 檢查 Email 是否為空
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Email 不可為空");
                return View(model);
            }


            // 檢查 Email 是否重複
            bool emailExists = await _context.MemberLists
                .AnyAsync(m => m.Email == model.Email);

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
            // 自動產生 MemberCode
            var random = new Random().Next(100, 999);
            model.MemberCode = "M" + DateTime.Now.ToString("yyyyMMddHHmmss") + random;

            // 密碼加密
            model.PasswordHash = HashPassword(model.PasswordHash);

            _context.MemberLists.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create");
        }

        // ==========================
        // 密碼加密方法S
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
