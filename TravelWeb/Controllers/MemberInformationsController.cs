using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.ViewModels;

namespace TravelWeb.Controllers
{
    public class MemberInformationsController : Controller
    {
        private readonly MemberSystemContext _context;

        public MemberInformationsController(MemberSystemContext context)
        {
            _context = context;
        }

        // ==========================
        // POST: Create (送出個人資料)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberInformation model, IFormFile? avatarFile)
        {
            // 🔥 預防針：把後端自己產生的欄位從驗證中移除，避免驗證失敗被擋下！
            ModelState.Remove("MemberId");
            ModelState.Remove("AvatarUrl");
            // (如果您的 Model 裡面還有其他不用填的欄位，請在這裡繼續 Remove)

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 自動產生 MemberId
            var member = await _context.MemberLists
                .FirstOrDefaultAsync(m => m.MemberCode == model.MemberCode);

            if (member != null && !string.IsNullOrEmpty(member.Email))
            {
                model.MemberId = GenerateMemberId(member.Email);
            }

            // 處理圖片上傳
            if (avatarFile != null && avatarFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                model.AvatarUrl = "/uploads/" + fileName;
            }
            else
            {
                // 如果沒上傳照片，可以給一張預設頭像 (視您的需求)
                model.AvatarUrl = "/images/default-avatar.png";
            }

            // 存入資料庫
            _context.MemberInformations.Add(model);
            await _context.SaveChangesAsync();

            // 🔥 修改這裡！填寫完畢後，應該跳轉到「登入頁面」或是「首頁」，不要再跳回 Create 了
            TempData["SuccessMessage"] = "資料建立成功，請登入！";
            return RedirectToAction("Login", "Auth");
        }

        // 自動產生 MemberId 的小工具
        private string GenerateMemberId(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return string.Empty;

            var parts = email.Split('@');
            string accountPart = parts[0];          // 1245
            string domainPart = parts[1];           // gmail.com

            string domainFirstChar = domainPart.Substring(0, 1).ToLower();  // g

            return "@" + accountPart + domainFirstChar;
        }

        [HttpGet]
        public IActionResult Create(string memberCode)
        {
            // 檢查有沒有帶著識別碼過來，沒有的話導回帳號註冊頁
            if (string.IsNullOrEmpty(memberCode))
            {
                return RedirectToAction("Create", "MemberAccount");
            }

            // 建立一個新的 Model 並預填好 MemberCode
            var model = new MemberInformation
            {
                MemberCode = memberCode
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MemberInformation model, IFormFile avatarFile)
        {
            if (id != model.MemberId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingData = await _context.MemberInformations
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (existingData == null)
            {
                return NotFound();
            }

            // 更新基本資料
            existingData.Name = model.Name;
            existingData.Status = model.Status;

            // 如果有上傳新圖片
            if (avatarFile != null && avatarFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                existingData.AvatarUrl = "/uploads/" + fileName;
            }

            _context.Update(existingData);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Profile(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
                return NotFound();

            var memberInfo = await _context.MemberInformations
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (memberInfo == null)
                return NotFound();

            var memberList = await _context.MemberLists
                .FirstOrDefaultAsync(m => m.MemberCode == memberInfo.MemberCode);

            if (memberList == null)
                return NotFound();

            // 🔥 取得登入資訊
            var loginMemberId = HttpContext.Session.GetString("MemberId");
            var role = HttpContext.Session.GetString("Role");

            bool canViewPrivateData = false;

            // 👤 本人
            if (loginMemberId == memberId)
                canViewPrivateData = true;

            // 👑 管理員
            if (role == "Admin" || role == "SuperAdmin")
                canViewPrivateData = true;

            var viewModel = new MemberProfileViewModel
            {
                MemberId = memberInfo.MemberId,
                Name = memberInfo.Name,
                Status = memberInfo.Status,
                AvatarUrl = memberInfo.AvatarUrl,

                // 🔥 只有允許時才給資料
                Email = canViewPrivateData ? memberList.Email : null,
                Phone = canViewPrivateData ? memberList.Phone : null
            };

            ViewBag.CanViewPrivateData = canViewPrivateData;

            return View(viewModel);
        }


    }
}
