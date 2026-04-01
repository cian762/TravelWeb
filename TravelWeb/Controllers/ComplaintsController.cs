//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TravelWeb.Models; // 請確認這裡的 Namespace 符合你的專案

//namespace TravelWeb.Controllers
//{
//    public class ComplaintsController : Controller
//    {
//        private readonly MemberSystemContext _context;

//        public ComplaintsController(MemberSystemContext context)
//        {
//            _context = context;
//        }

//        // 🌟 權限檢查共用方法 (防堵一般會員或未登入者偷看)
//        private bool IsAdmin()
//        {
//            return HttpContext.Session.GetString("Role") == "Admin";
//        }

//        // ==========================================
//        // 1. 客訴總覽清單
//        // ==========================================
//        public async Task<IActionResult> Index()
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

//            // 撈取客訴，並 Join Complaint_Record 來判斷處理狀態
//            var complaints = await _context.MemberComplaints
//                .Include(c => c.ComplaintRecord) // 假設你有設定關聯導覽屬性
//                .OrderByDescending(c => c.CreatedAt)
//                .ToListAsync();

//            return View(complaints);
//        }

//        // ==========================================
//        // 2. 檢視單筆客訴 + 回覆介面
//        // ==========================================
//        public async Task<IActionResult> Details(string id)
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
//            if (string.IsNullOrEmpty(id)) return NotFound();

//            var complaint = await _context.MemberComplaints
//                .Include(c => c.ComplaintRecord)
//                .FirstOrDefaultAsync(m => m.ComplaintId == id);

//            if (complaint == null) return NotFound();

//            return View(complaint);
//        }

//        // ==========================================
//        // 3. 處理回覆表單 (POST)
//        // ==========================================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Reply(string ComplaintId, string Status, string Compensation, string MemberId)
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

//            string adminId = HttpContext.Session.GetString("UserCode");

//            // 找尋對應的 MemberCode (因為 Complaint_Record 需要 MemberCode)
//            var memberInfo = await _context.MemberInformations.FirstOrDefaultAsync(m => m.MemberId == MemberId);
//            string memberCode = memberInfo != null ? memberInfo.MemberCode : "未知會員";

//            // 檢查是否已經有回覆紀錄了
//            var record = await _context.ComplaintRecords.FirstOrDefaultAsync(r => r.ComplaintId == ComplaintId);

//            if (record == null)
//            {
//                // 新增紀錄
//                record = new ComplaintRecord
//                {
//                    ComplaintId = ComplaintId,
//                    AdminId = adminId,
//                    MemberCode = memberCode,
//                    Status = Status,
//                    Compensation = Compensation ?? "" // 若沒填補償則給空字串
//                };
//                _context.ComplaintRecords.Add(record);
//            }
//            else
//            {
//                // 更新已存在的紀錄
//                record.AdminId = adminId;
//                record.Status = Status;
//                record.Compensation = Compensation ?? "";
//                _context.Update(record);
//            }

//            await _context.SaveChangesAsync();

//            TempData["SuccessMsg"] = "客訴處理紀錄已成功儲存！";
//            return RedirectToAction("Details", new { id = ComplaintId });
//        }

//        // ==========================================
//        // 4. 檢視所有處理紀錄
//        // ==========================================
//        public async Task<IActionResult> Records()
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

//            var records = await _context.ComplaintRecords
//                .Include(r => r.ComplaintIdNavigation) // 這裡替換成你 EF Core 實際產生的導覽屬性名稱
//                .OrderByDescending(r => r.ComplaintIdNavigation.CreatedAt)
//                .ToListAsync();

//            return View(records);
//        }
//    }
//}