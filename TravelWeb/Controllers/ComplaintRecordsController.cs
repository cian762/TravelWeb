using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
    public class ComplaintRecordsController : Controller
    {
        private readonly MemberSystemContext _context;

        public ComplaintRecordsController(MemberSystemContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET: 清單頁面 (包含圖表資料與下拉選單)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // 權限檢查
            var userCode = HttpContext.Session.GetString("UserCode");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userCode)) return RedirectToAction("Login", "Auth");

            IQueryable<ComplaintRecord> query = _context.ComplaintRecords;

            // 依據角色過濾資料
            if (role == "Member")
            {
                query = query.Where(c => c.MemberCode == userCode);
            }

            // 取得清單資料 (此時還不會把 Compensation 顯示在畫面上，這交給 View 處理)
            var records = await query.OrderByDescending(c => c.ComplaintId).ToListAsync();

            // ----------------------------------------
            // 📊 準備圖表資料：計算「每月」客訴數量
            // ----------------------------------------
            // 因為我們的 ID 格式是 YYYYMMDD+流水號，所以擷取第 5、6 碼就是月份 (Substring(4, 2))
            var monthlyData = records
                .Where(c => !string.IsNullOrEmpty(c.ComplaintId) && c.ComplaintId.Length >= 8)
                .GroupBy(c => c.ComplaintId.Substring(4, 2)) // 取出 MM (月份)
                .OrderBy(g => g.Key)
                .Select(g => new { Month = g.Key + "月", Count = g.Count() })
                .ToList();

            // 將圖表資料轉成 JSON，方便前端 Chart.js 讀取
            ViewBag.ChartLabels = JsonSerializer.Serialize(monthlyData.Select(x => x.Month));
            ViewBag.ChartCounts = JsonSerializer.Serialize(monthlyData.Select(x => x.Count));

            // ----------------------------------------
            // 🔽 準備下拉選單 (供前端篩選或修改使用)
            // ----------------------------------------
            await PrepareDropdownListsAsync();

            ViewBag.Role = role; // 讓前端判斷要不要顯示「編輯/指派」按鈕

            return View(records);
        }

        // ==========================================
        // 2. GET: 詳細頁面 (圖表式顯示詳細內容)
        // ==========================================
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var userCode = HttpContext.Session.GetString("UserCode");
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(userCode)) return RedirectToAction("Login", "Auth");

            var record = await _context.ComplaintRecords.FirstOrDefaultAsync(m => m.ComplaintId == id);
            if (record == null) return NotFound();

            // 防呆：會員只能看自己的
            if (role == "Member" && record.MemberCode != userCode) return RedirectToAction("Index");

            return View(record);
        }

        // ==========================================
        // 3. GET: 新增客訴
        // ==========================================
        public async Task<IActionResult> Create()
        {
            var userCode = HttpContext.Session.GetString("UserCode");
            if (string.IsNullOrEmpty(userCode)) return RedirectToAction("Login", "Auth");

            // 準備下拉選單給 View 使用
            await PrepareDropdownListsAsync();

            return View();
        }

        // ==========================================
        // 4. POST: 儲存新增的客訴 (自動產生 ID)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintRecord model)
        {
            var userCode = HttpContext.Session.GetString("UserCode");

            // 排除系統自動產生的欄位驗證
            ModelState.Remove("ComplaintId");
            ModelState.Remove("MemberCode");

            if (!ModelState.IsValid)
            {
                await PrepareDropdownListsAsync();
                return View(model);
            }

            // 💡 自動產生 ComplaintId (格式：yyyyMMdd + 3碼流水號，例如 20231027001)
            model.ComplaintId = await GenerateComplaintIdAsync();

            // 綁定當前登入的會員
            model.MemberCode = userCode;

            // 如果沒有選狀態，預設為「已檢視」或「已立案」
            if (string.IsNullOrEmpty(model.Status))
            {
                model.Status = "已檢視";
            }

            _context.ComplaintRecords.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 共用方法：準備下拉選單 (Status & AdminId)
        // ==========================================
        private async Task PrepareDropdownListsAsync()
        {
            // 1. 狀態下拉選單 (4種狀態)
            var statusList = new List<string> { "已檢視", "已審核", "已立案", "已處理" };
            ViewBag.StatusList = new SelectList(statusList);

            // 2. 管理員下拉選單 (從 Administrators 資料表讀取)
            // 假設您的 Administrator 表有 AdminId 欄位 (您可以依需求加入 Name 欄位來顯示名字)
            var admins = await _context.Administrators.ToListAsync();

            // SelectList(資料來源, "存入資料庫的值", "顯示在網頁上的字")
            ViewBag.AdminList = new SelectList(admins, "AdminId", "AdminId");
        }

        // ==========================================
        // 共用方法：自動生成 日期+流水號 ID
        // ==========================================
        private async Task<string> GenerateComplaintIdAsync()
        {
            // 取得今天的日期字串 (例如：20231027)
            string todayPrefix = DateTime.Now.ToString("yyyyMMdd");

            // 去資料庫找「今天」最新的一筆客訴編號
            var lastRecord = await _context.ComplaintRecords
                .Where(c => c.ComplaintId.StartsWith(todayPrefix))
                .OrderByDescending(c => c.ComplaintId)
                .FirstOrDefaultAsync();

            int sequence = 1;

            if (lastRecord != null && lastRecord.ComplaintId.Length >= 11)
            {
                // 擷取最後 3 碼轉成數字，然後 +1
                string lastSeqStr = lastRecord.ComplaintId.Substring(8, 3);
                if (int.TryParse(lastSeqStr, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            // 組合：日期 + 3位數補零的流水號 (例如：20231027001)
            return $"{todayPrefix}{sequence:D3}";
        }
    }
}