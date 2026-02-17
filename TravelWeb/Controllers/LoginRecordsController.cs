using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class LoginRecordsController : Controller
    {
        private readonly MemberSystemContext _context;

        public LoginRecordsController(MemberSystemContext context)
        {
            _context = context;
        }

        // 管理員登入紀錄頁面 + 搜尋功能
        public async Task<IActionResult> Index(
            string? memberCode,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.LogInRecords
                                .Include(l => l.MemberCodeNavigation)
                                .AsQueryable();

            // 🔍 依會員識別碼搜尋
            if (!string.IsNullOrEmpty(memberCode))
            {
                query = query.Where(l => l.MemberCode == memberCode);
            }

            // 🔍 依開始時間搜尋
            if (startDate.HasValue)
            {
                query = query.Where(l => l.LoginAt >= startDate.Value);
            }

            // 🔍 依結束時間搜尋
            if (endDate.HasValue)
            {
                query = query.Where(l => l.LoginAt <= endDate.Value);
            }

            var result = await query
                .OrderByDescending(l => l.LoginAt)
                .ToListAsync();

            return View(result);
        }
    }
}
