using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.Filters;


namespace TravelWeb.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly MemberSystemContext _context;

        public ComplaintsController(MemberSystemContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("Role");
            // 只有 Admin 或 SuperAdmin 可以看客訴紀錄
            if (role != "Admin" && role != "SuperAdmin")
            {
                context.Result = RedirectToAction("Index", "Home");
            }
            base.OnActionExecuting(context);
        }

        // 🌟 權限檢查共用方法 (防堵一般會員或未登入者偷看)
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // ==========================================
        // 1. 客訴總覽清單
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // 🔐 安全檢查：如果是 Admin 才能看
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin" && role != "SuperAdmin")
            {
                return RedirectToAction("Index", "Home");
            }

            // 🚀 執行查詢：抓取客訴主表，並「同時抓取」關聯的細節表與會員表
            var complaints = await _context.MemberComplaints
                .Include(c => c.Complaint) // 抓取 ComplaintRecord (包含 Status, AdminId)
                .Include(c => c.Member)    // 抓取 MemberInformation (包含 MemberCode)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(complaints);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var complaint = await _context.MemberComplaints
                   .Include(m => m.Complaint) 
                   .Include(m => m.Member)   
                   .FirstOrDefaultAsync(m => m.ComplaintId == id);

            if (complaint == null) return NotFound();

            return View(complaint);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(string ComplaintId, string Status, string Compensation, string MemberId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            string adminId = HttpContext.Session.GetString("UserCode");

            var memberInfo = await _context.MemberInformations.FirstOrDefaultAsync(m => m.MemberId == MemberId);
            string memberCode = memberInfo != null ? memberInfo.MemberCode : "未知會員";

            var record = await _context.ComplaintRecords.FirstOrDefaultAsync(r => r.ComplaintId == ComplaintId);

            if (record == null)
            {
                record = new ComplaintRecord
                {
                    ComplaintId = ComplaintId,
                    AdminId = adminId,
                    MemberCode = memberCode,
                    Status = Status,
                    Compensation = Compensation ?? "" 
                };
                _context.ComplaintRecords.Add(record);
            }
            else
            {
                record.AdminId = adminId;
                record.Status = Status;
                record.Compensation = Compensation ?? "";
                _context.Update(record);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = "客訴處理紀錄已成功儲存！";
            return RedirectToAction("Details", new { id = ComplaintId });
        }

        public async Task<IActionResult> Records()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var records = await _context.ComplaintRecords
               .Include(r => r.MemberComplaint) 
               .OrderByDescending(r => r.MemberComplaint.CreatedAt)
               .ToListAsync();

            return View(records);
        }
    }
}