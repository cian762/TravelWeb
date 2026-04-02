using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.Filters;


namespace TravelWeb.Controllers
{
    [AdminAuthorize]
    public class LoginRecordsController : Controller
    {
        private readonly MemberSystemContext _context;

        public LoginRecordsController(MemberSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? memberCode,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.LogInRecords
                                .Include(l => l.MemberCodeNavigation)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(memberCode))
            {
                query = query.Where(l => l.MemberCode == memberCode);
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.LoginAt >= startDate.Value);
            }

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
