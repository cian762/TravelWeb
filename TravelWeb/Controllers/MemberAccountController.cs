using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TravelWeb.Models;
using TravelWeb.Filters;

namespace TravelWeb.Controllers
{
    [AdminAuthorize]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberList model, string confirmPassword)
        {
            ModelState.Remove("MemberCode");
            ModelState.Remove("Authorizations");
            ModelState.Remove("ComplaintRecords");
            ModelState.Remove("LogInRecords");
            ModelState.Remove("MemberInformations");

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ModelState.AddModelError("", "表單驗證失敗，原因：" + string.Join(" / ", errorMessages));
                return View(model);
            }

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

            var random = new Random().Next(100, 999);
            model.MemberCode = "M" + DateTime.Now.ToString("yyyyMMddHHmmss") + random;
            model.PasswordHash = HashPassword(model.PasswordHash);

            try
            {
                _context.MemberLists.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Create", "MemberInformations", new { id = model.MemberCode });
            }
            catch (Exception ex)
            {
                string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "資料庫寫入失敗，真實原因：" + realError);

                return View(model);
            }
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
