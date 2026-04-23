using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.ViewModels;
using TravelWeb.Filters;

namespace TravelWeb.Controllers
{
    [AdminAuthorize]
    public class MemberInformationsController : Controller
    {
        private readonly MemberSystemContext _context;

        public MemberInformationsController(MemberSystemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var sessionUserCode = HttpContext.Session.GetString("UserCode");
            if (string.IsNullOrEmpty(sessionUserCode))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new MemberInformation { MemberCode = sessionUserCode };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MemberInformation model, IFormFile? avatarFile)
        {
            ModelState.Remove("MemberId");
            ModelState.Remove("Status");
            ModelState.Remove("AvatarUrl");

            ModelState.Remove("MemberCodeNavigation"); 
            ModelState.Remove("Member"); 
            ModelState.Remove("Followeds");
            ModelState.Remove("Followers");
            ModelState.Remove("MemberComplaints");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var memberAccount = await _context.MemberLists
                .FirstOrDefaultAsync(m => m.MemberCode == model.MemberCode);

            if (memberAccount != null && !string.IsNullOrEmpty(memberAccount.Email))
            {
                var emailParts = memberAccount.Email.Split('@');
                string accountPrefix = emailParts[0];
                string randomNum = new Random().Next(100, 999).ToString(); 

                model.MemberId = accountPrefix + randomNum;
            }
            else
            {
                model.MemberId = "User" + new Random().Next(1000, 9999).ToString();
            }

            model.Status = "正常";

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
                model.AvatarUrl = "/images/default-avatar.png";
            }

            _context.MemberInformations.Add(model);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserCode", model.MemberCode);
            HttpContext.Session.SetString("MemberId", model.MemberId);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MemberInformation model, IFormFile avatarFile)
        {
            var sessionMemberId = HttpContext.Session.GetString("MemberId");
            if (id != sessionMemberId)
            {
                return Forbid(); 
            }

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

            existingData.Name = model.Name;
            existingData.Status = model.Status;

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

            var loginMemberId = HttpContext.Session.GetString("MemberId");
            var role = HttpContext.Session.GetString("Role");

            bool canViewPrivateData = (loginMemberId == memberId || role == "Admin" || role == "SuperAdmin");


            if (loginMemberId == memberId)
                canViewPrivateData = true;

            if (role == "Admin" || role == "SuperAdmin")
                canViewPrivateData = true;

            var viewModel = new MemberProfileViewModel
            {
                MemberId = memberInfo.MemberId,
                Name = memberInfo.Name,
                Status = memberInfo.Status,
                AvatarUrl = memberInfo.AvatarUrl,

                Email = canViewPrivateData ? memberList.Email : null,
                Phone = canViewPrivateData ? memberList.Phone : null
            };

            ViewBag.CanViewPrivateData = canViewPrivateData;

            return View(viewModel);
        }


    }
}
