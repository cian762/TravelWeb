using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.ViewModels;
using TravelWeb.Filters;

namespace TravelWeb.Controllers
{
    [AdminAuthorize]
    public class MemberManageController : Controller
    {
        private readonly MemberSystemContext _context;

        public MemberManageController(MemberSystemContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "SuperAdmin")
            {
                context.Result = RedirectToAction("Index", "Home");
            }
            base.OnActionExecuting(context);
        }
        public async Task<IActionResult> Index(string keyword, string gender, string status, int? birthYear, int? birthMonth)
        {
            var query = _context.MemberLists.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(m =>
                    m.MemberCode.Contains(keyword) ||
                    m.Email.Contains(keyword) ||
                    m.MemberInformations.Any(info => info.MemberId.Contains(keyword) || info.Name.Contains(keyword))
                );
            }

            if (!string.IsNullOrWhiteSpace(gender))
            {
                if (byte.TryParse(gender, out byte genderValue))
                {
                    query = query.Where(m => m.MemberInformations.Any(info => info.Gender == genderValue));
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.MemberInformations.Any(info => info.Status == status));
            }

            if (birthYear.HasValue)
            {
                query = query.Where(m => m.MemberInformations.Any(info =>
                    info.BirthDate.HasValue && info.BirthDate.Value.Year == birthYear.Value));
            }

            if (birthMonth.HasValue)
            {
                query = query.Where(m => m.MemberInformations.Any(info =>
                    info.BirthDate.HasValue && info.BirthDate.Value.Month == birthMonth.Value));
            }

            var memberList = await query.Select(m => new MemberDetailsViewModel
            {
                MemberCode = m.MemberCode,
                Email = m.Email,

                MemberId = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().MemberId : "尚未建立",
                Name = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().Name : "尚未填寫",
                Status = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().Status : "未知",

                LastLoginTime = m.LogInRecords.OrderByDescending(log => log.LoginAt).FirstOrDefault() != null
                              ? m.LogInRecords.OrderByDescending(log => log.LoginAt).FirstOrDefault().LoginAt
                              : (DateTime?)null
            })
            .OrderByDescending(x => x.MemberCode)
            .ToListAsync();

            PrepareDropdowns(gender, status, birthYear, birthMonth);

            ViewBag.Keyword = keyword;

            return View(memberList);
        }

        private void PrepareDropdowns(string selectedGender, string selectedStatus, int? selectedYear, int? selectedMonth)
        {
            var genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "男" },
                new SelectListItem { Value = "2", Text = "女" }
            };
            ViewBag.GenderList = new SelectList(genders, "Value", "Text", selectedGender);

            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "正常", Text = "正常" },
                new SelectListItem { Value = "停權", Text = "停權" },
                new SelectListItem { Value = "警示", Text = "警示" }
            };
            ViewBag.StatusList = new SelectList(statuses, "Value", "Text", selectedStatus);

            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(1920, currentYear - 1920 + 1)
                .OrderByDescending(y => y) 
                .Select(y => new SelectListItem { Value = y.ToString(), Text = $"{y} 年" })
                .ToList();
            ViewBag.YearList = new SelectList(years, "Value", "Text", selectedYear);

            var months = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem { Value = m.ToString(), Text = $"{m} 月" })
                .ToList();
            ViewBag.MonthList = new SelectList(months, "Value", "Text", selectedMonth);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .Include(m => m.LogInRecords)
                .FirstOrDefaultAsync(m => m.MemberCode == id);

            if (member == null) return NotFound();

            var info = member.MemberInformations.FirstOrDefault();

            bool isAdmin = await _context.Administrators.AnyAsync(a => a.Email == member.Email);

            string genderStr = "未設定";
            if (info != null && info.Gender.HasValue)
            {
                genderStr = info.Gender.Value == 1 ? "男" : "女";
            }

            var viewModel = new MemberDetailsViewModel
            {
                MemberCode = member.MemberCode,
                Email = member.Email,
                Phone = member.Phone,
                PasswordHash = member.PasswordHash, 

                MemberId = info?.MemberId ?? "無",
                Name = info?.Name ?? "未填寫",
                Status = info?.Status ?? "正常",
                BirthDate = info?.BirthDate,
                AvatarUrl = info?.AvatarUrl,
                GenderText = genderStr,

                LastLoginTime = member.LogInRecords.OrderByDescending(l => l.LoginAt).FirstOrDefault()?.LoginAt,
                IsAlreadyAdmin = isAdmin
            };

            ViewBag.StatusList = new SelectList(new[] { "正常", "停權", "警示" }, viewModel.Status);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsAdmin(string memberCode)
        {
            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .FirstOrDefaultAsync(m => m.MemberCode == memberCode);

            if (member == null) return NotFound();

            if (await _context.Administrators.AnyAsync(a => a.Email == member.Email))
            {
                TempData["Message"] = "此會員已經是管理員！";
                return RedirectToAction(nameof(Details), new { id = memberCode });
            }

            string newAdminId = member.MemberCode.StartsWith("M")
                ? "G" + member.MemberCode.Substring(1)
                : "G" + member.MemberCode;

            var info = member.MemberInformations.FirstOrDefault();

            var newAdmin = new Administrator
            {
                AdminId = newAdminId,
                Email = member.Email,
                PasswordHash = member.PasswordHash, 
                Phone = member.Phone,
                Name = info?.Name ?? "未填寫管理員姓名"
            };

            _context.Administrators.Add(newAdmin);
            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = $"成功將 {newAdmin.Name} 設為管理員！管理員帳號為：{newAdminId}";
            return RedirectToAction(nameof(Details), new { id = memberCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string memberCode, string newStatus, string blockReason)
        {
            var info = await _context.MemberInformations.FirstOrDefaultAsync(m => m.MemberCode == memberCode);
            if (info == null) return NotFound();

            info.Status = newStatus;

            if (newStatus == "停權")
            {
                bool isAlreadyBlocked = await _context.Blockeds.AnyAsync(b => b.MemberId == info.MemberId);
                if (!isAlreadyBlocked)
                {
                    var blockedRecord = new Blocked
                    {
                        MemberId = info.MemberId,
                        BlockedId = "B" + DateTime.Now.ToString("yyyyMMddHHmmss"), 
                        Reason = string.IsNullOrEmpty(blockReason) ? "管理員手動停權" : blockReason
                    };
                    _context.Blockeds.Add(blockedRecord);
                }
            }
            else
            {
                var blockRecord = await _context.Blockeds.FirstOrDefaultAsync(b => b.MemberId == info.MemberId);
                if (blockRecord != null)
                {
                    _context.Blockeds.Remove(blockRecord);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMsg"] = $"已成功將狀態更改為：{newStatus}";

            return RedirectToAction(nameof(Details), new { id = memberCode });
        }

        // ==========================================
        // 4. GET & POST: 編輯會員資料
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(string id) // id 是 MemberCode
        {
            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .FirstOrDefaultAsync(m => m.MemberCode == id);

            if (member == null) return NotFound();
            var info = member.MemberInformations.FirstOrDefault();

            var viewModel = new MemberEditViewModel
            {
                MemberCode = member.MemberCode,
                MemberId = info?.MemberId,
                Status = info?.Status,
                Email = member.Email,
                Phone = member.Phone,
                Name = info?.Name,
                Gender = info?.Gender,
                BirthDate = info?.BirthDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MemberEditViewModel model)
        {
            // 不驗證唯讀欄位
            ModelState.Remove("MemberCode");
            ModelState.Remove("MemberId");
            ModelState.Remove("Status");

            if (!ModelState.IsValid) return View(model);

            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .FirstOrDefaultAsync(m => m.MemberCode == model.MemberCode);

            if (member == null) return NotFound();

            // 更新 MemberList 資料
            member.Email = model.Email;
            member.Phone = model.Phone;

            // 更新 MemberInformation 資料
            var info = member.MemberInformations.FirstOrDefault();
            if (info != null)
            {
                info.Name = model.Name;
                info.Gender = model.Gender;
                info.BirthDate = model.BirthDate;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMsg"] = "會員資料編輯成功！";

            return RedirectToAction(nameof(Details), new { id = model.MemberCode });
        }
    }
}
