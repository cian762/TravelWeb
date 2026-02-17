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
        // GET: Create
        // ==========================
        public IActionResult Create(string memberCode)
        {
            // 建立新物件並帶入會員識別碼
            var model = new MemberInformation
            {
                MemberCode = memberCode
            };

            return View(model);
        }

        private string GenerateMemberId(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return null;

            var parts = email.Split('@');
            string accountPart = parts[0];          // 1245
            string domainPart = parts[1];           // gmail.com

            string domainFirstChar = domainPart.Substring(0, 1).ToLower();  // g

            return "@" + accountPart + domainFirstChar;
        }

        // ==========================
        // POST: Create
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberInformation model, IFormFile avatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 自動產生 MemberId
            var member = await _context.MemberLists
    .FirstOrDefaultAsync(m => m.MemberCode == model.MemberCode);

            if (member != null)
            {
                model.MemberId = GenerateMemberId(member.Email);
            }


            // 如果有上傳圖片
            if (avatarFile!= null && avatarFile.Length > 0)
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


            _context.MemberInformations.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(
    "Create",
    "MemberInformations",
    new { memberCode = model.MemberCode }
);
        }

        // GET: MemberInformation/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberInfo = await _context.MemberInformations.FindAsync(id);
            if (memberInfo == null)
            {
                return NotFound();
            }

            return View(memberInfo);
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
