using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.ViewModels;

namespace TravelWeb.Controllers
{
    public class MemberManageController : Controller
    {
        private readonly MemberSystemContext _context;

        public MemberManageController(MemberSystemContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: 會員清單檢視頁面
        // 參數接收從前端傳來的「搜尋關鍵字」與「下拉選單過濾條件」
        // ==========================================
        public async Task<IActionResult> Index(string keyword, string gender, string status, int? birthYear, int? birthMonth)
        {
            // 1. 建立基底查詢 (先不真正去資料庫撈資料，只把語法準備好)
            // 從 MemberLists 作為起點，因為它是最核心的表
            var query = _context.MemberLists.AsQueryable();

            // ----------------------------------------
            // 🔍 2. 關鍵字搜尋 (MemberCode, Email, MemberId, Name)
            // ----------------------------------------
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(m =>
                    m.MemberCode.Contains(keyword) ||
                    m.Email.Contains(keyword) ||
                    // 因為 MemberInformations 是關聯表，用 Any 來跨表搜尋
                    m.MemberInformations.Any(info => info.MemberId.Contains(keyword) || info.Name.Contains(keyword))
                );
            }

            // ----------------------------------------
            // 🔽 3. 下拉選單過濾條件
            // ----------------------------------------
            // 🔥 修正：性別過濾 (處理 string 轉 byte? 的問題)
            if (!string.IsNullOrWhiteSpace(gender))
            {
                // 嘗試將前端傳來的字串 ("1" 或 "2") 轉換成 byte
                if (byte.TryParse(gender, out byte genderValue))
                {
                    // 轉換成功後，再去資料庫比對 (byte? == byte 是可以的)
                    query = query.Where(m => m.MemberInformations.Any(info => info.Gender == genderValue));
                }
            }

            // 狀態過濾 (正常/停權/警示)
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.MemberInformations.Any(info => info.Status == status));
            }

            // 生日(年份)過濾
            if (birthYear.HasValue)
            {
                query = query.Where(m => m.MemberInformations.Any(info =>
                    info.BirthDate.HasValue && info.BirthDate.Value.Year == birthYear.Value));
            }

            // 生日(月份)過濾
            if (birthMonth.HasValue)
            {
                query = query.Where(m => m.MemberInformations.Any(info =>
                    info.BirthDate.HasValue && info.BirthDate.Value.Month == birthMonth.Value));
            }

            // ----------------------------------------
            // 🚀 4. 投影到 ViewModel 並執行查詢 (這步才會真的去資料庫撈資料)
            // ----------------------------------------
            var memberList = await query.Select(m => new MemberDetailsViewModel
            {
                MemberCode = m.MemberCode,
                Email = m.Email,

                // 抓取 MemberInformation 第一筆對應的資料 (如果沒填寫就給預設字串)
                MemberId = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().MemberId : "尚未建立",
                Name = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().Name : "尚未填寫",
                Status = m.MemberInformations.FirstOrDefault() != null ? m.MemberInformations.FirstOrDefault().Status : "未知",

                // 抓取 LogInRecord 裡面「日期最新 (OrderByDescending)」的那一筆的登入時間
                LastLoginTime = m.LogInRecords.OrderByDescending(log => log.LoginAt).FirstOrDefault() != null
                              ? m.LogInRecords.OrderByDescending(log => log.LoginAt).FirstOrDefault().LoginAt
                              : (DateTime?)null
            })
            // 預設依照 MemberCode 降冪排序 (最新註冊的在最上面)
            .OrderByDescending(x => x.MemberCode)
            .ToListAsync();

            // ----------------------------------------
            // 🎨 5. 準備下拉選單丟給前端 View 顯示
            // ----------------------------------------
            PrepareDropdowns(gender, status, birthYear, birthMonth);

            // 把關鍵字也傳回畫面，讓輸入框保留使用者剛剛搜尋的字
            ViewBag.Keyword = keyword;

            return View(memberList);
        }

        // ==========================================
        // 共用方法：準備各種過濾用的下拉選單
        // ==========================================
        private void PrepareDropdowns(string selectedGender, string selectedStatus, int? selectedYear, int? selectedMonth)
        {
            // 1. 性別下拉 (1=男 / 2=女)
            var genders = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "男" },
                new SelectListItem { Value = "2", Text = "女" }
            };
            ViewBag.GenderList = new SelectList(genders, "Value", "Text", selectedGender);

            // 2. 狀態下拉 (正常/停權/警示)
            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "正常", Text = "正常" },
                new SelectListItem { Value = "停權", Text = "停權" },
                new SelectListItem { Value = "警示", Text = "警示" }
            };
            ViewBag.StatusList = new SelectList(statuses, "Value", "Text", selectedStatus);

            // 3. 生日(年份)下拉：從 1920 年到今年
            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(1920, currentYear - 1920 + 1)
                .OrderByDescending(y => y) // 年份從大排到小
                .Select(y => new SelectListItem { Value = y.ToString(), Text = $"{y} 年" })
                .ToList();
            ViewBag.YearList = new SelectList(years, "Value", "Text", selectedYear);

            // 4. 生日(月份)下拉：1 到 12 月
            var months = Enumerable.Range(1, 12)
                .Select(m => new SelectListItem { Value = m.ToString(), Text = $"{m} 月" })
                .ToList();
            ViewBag.MonthList = new SelectList(months, "Value", "Text", selectedMonth);
        }

        // ==========================================
        // 1. GET: 會員詳細資料檢視
        // ==========================================
        public async Task<IActionResult> Details(string id) // id 是 MemberCode
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // 抓取 MemberList，並一併將關聯的 Information 和 LogInRecords 抓出來
            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .Include(m => m.LogInRecords)
                .FirstOrDefaultAsync(m => m.MemberCode == id);

            if (member == null) return NotFound();

            var info = member.MemberInformations.FirstOrDefault();

            // 檢查是否已經是管理員 (用 Email 去管理員表查)
            bool isAdmin = await _context.Administrators.AnyAsync(a => a.Email == member.Email);

            // 轉換性別顯示
            string genderStr = "未設定";
            if (info != null && info.Gender.HasValue)
            {
                genderStr = info.Gender.Value == 1 ? "男" : "女"; // 假設 1=男, 0或2=女
            }

            var viewModel = new MemberDetailsViewModel
            {
                MemberCode = member.MemberCode,
                Email = member.Email,
                Phone = member.Phone,
                PasswordHash = member.PasswordHash, // ⚠️ 只能顯示 Hash 值，無法反解密

                MemberId = info?.MemberId ?? "無",
                Name = info?.Name ?? "未填寫",
                Status = info?.Status ?? "正常",
                BirthDate = info?.BirthDate,
                AvatarUrl = info?.AvatarUrl,
                GenderText = genderStr,

                LastLoginTime = member.LogInRecords.OrderByDescending(l => l.LoginAt).FirstOrDefault()?.LoginAt,
                IsAlreadyAdmin = isAdmin
            };

            // 準備狀態下拉選單給 View 使用
            ViewBag.StatusList = new SelectList(new[] { "正常", "停權", "警示" }, viewModel.Status);

            return View(viewModel);
        }

        // ==========================================
        // 2. POST: 設為管理員
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsAdmin(string memberCode)
        {
            var member = await _context.MemberLists
                .Include(m => m.MemberInformations)
                .FirstOrDefaultAsync(m => m.MemberCode == memberCode);

            if (member == null) return NotFound();

            // 防呆：確認是否已經是管理員
            if (await _context.Administrators.AnyAsync(a => a.Email == member.Email))
            {
                TempData["Message"] = "此會員已經是管理員！";
                return RedirectToAction(nameof(Details), new { id = memberCode });
            }

            // 💡 規則：AdminId 生成規則為 MemberCode 開頭的 M 變更為 G
            // 例如：M20231027123 -> G20231027123
            string newAdminId = member.MemberCode.StartsWith("M")
                ? "G" + member.MemberCode.Substring(1)
                : "G" + member.MemberCode;

            var info = member.MemberInformations.FirstOrDefault();

            // 建立管理員資料
            var newAdmin = new Administrator
            {
                AdminId = newAdminId,
                Email = member.Email,
                PasswordHash = member.PasswordHash, // 沿用原密碼
                Phone = member.Phone,
                Name = info?.Name ?? "未填寫管理員姓名"
            };

            _context.Administrators.Add(newAdmin);
            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = $"成功將 {newAdmin.Name} 設為管理員！管理員帳號為：{newAdminId}";
            return RedirectToAction(nameof(Details), new { id = memberCode });
        }

        // ==========================================
        // 3. POST: 更改狀態 (包含停權/封禁邏輯)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string memberCode, string newStatus, string blockReason)
        {
            var info = await _context.MemberInformations.FirstOrDefaultAsync(m => m.MemberCode == memberCode);
            if (info == null) return NotFound();

            info.Status = newStatus;

            // 🔥 如果狀態改為「停權」，寫入 Blocked (封禁) 資料表
            if (newStatus == "停權")
            {
                // 檢查是否已經在封禁名單中，避免重複寫入報錯
                bool isAlreadyBlocked = await _context.Blockeds.AnyAsync(b => b.MemberId == info.MemberId);
                if (!isAlreadyBlocked)
                {
                    var blockedRecord = new Blocked
                    {
                        MemberId = info.MemberId,
                        BlockedId = "B" + DateTime.Now.ToString("yyyyMMddHHmmss"), // 自動生成封鎖編號
                        Reason = string.IsNullOrEmpty(blockReason) ? "管理員手動停權" : blockReason
                    };
                    _context.Blockeds.Add(blockedRecord);
                }
            }
            else
            {
                // 如果改回正常或警示，可以選擇把 Blocked 紀錄刪除 (解除封禁)
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
