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
        // GET: 填寫個人資料頁面
        // ==========================
        // 🔥 注意：參數名稱改為 id，配合系統預設路由
        [HttpGet]
        public IActionResult Create(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Login", "Auth"); // 沒帶入會員代碼就踢回登入頁
            }

            // 建立新物件並帶入上一頁註冊好的 會員識別碼(MemberCode)
            var model = new MemberInformation
            {
                MemberCode = id
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
        // POST: 儲存個人資料
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberInformation model, IFormFile? avatarFile)
        {
            // 1. 排除後端自動產生的欄位
            ModelState.Remove("MemberId");
            ModelState.Remove("Status");
            ModelState.Remove("AvatarUrl");

            // 🔥 2. 關鍵救援：排除所有 EF Core 產生的關聯屬性！
            ModelState.Remove("MemberCodeNavigation"); // 關聯到 MemberList
            ModelState.Remove("Member");               // 關聯到 Blocked
            ModelState.Remove("Followeds");            // 關聯到 追蹤表
            ModelState.Remove("Followers");            // 關聯到 被追蹤表
            ModelState.Remove("MemberComplaints");     // 關聯到 客訴表

            if (!ModelState.IsValid)
            {
                // 驗證失敗退回畫面
                return View(model);
            }

            // 1. 自動產生 MemberId (信箱@前字串 + 隨機3位數)
            var memberAccount = await _context.MemberLists
    .FirstOrDefaultAsync(m => m.MemberCode == model.MemberCode);

            if (memberAccount != null && !string.IsNullOrEmpty(memberAccount.Email))
            {
                var emailParts = memberAccount.Email.Split('@');
                string accountPrefix = emailParts[0]; // 取得 @ 前面的字串
                string randomNum = new Random().Next(100, 999).ToString(); // 隨機 3 位數

                model.MemberId = accountPrefix + randomNum; // 組合！
            }
            else
            {
                // 防呆機制：萬一找不到信箱，給一個預設格式
                model.MemberId = "User" + new Random().Next(1000, 9999).ToString();
            }

            // 2. 自動寫入狀態
            model.Status = "正常";

            // 3. 處理圖片上傳 (AvatarUrl)
            if (avatarFile != null && avatarFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // 檢查資料夾是否存在，沒有就自動建立
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 產生唯一的檔名避免覆蓋
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
                // 如果沒有上傳圖片，可以給一張預設頭像
                model.AvatarUrl = "/images/default-avatar.png";
            }

            // 4. 寫入 Member_Information 資料庫
            _context.MemberInformations.Add(model);
            await _context.SaveChangesAsync();

            // 5. 🔥 成功儲存！跳轉到首頁 (Home Controller 的 Index Action)
            // 您也可以把剛產生好的 MemberId 存進 Session 方便首頁讀取
            HttpContext.Session.SetString("UserCode", model.MemberCode);
            HttpContext.Session.SetString("MemberId", model.MemberId);

            return RedirectToAction("Index", "Home");
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
