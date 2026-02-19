using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;

namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class AttractionTicketController : Controller 
    {
        private readonly AttractionsContext _context; 

        public AttractionTicketController(AttractionsContext context)
        {
            _context = context;
        }

        // 只保留這一個 Index 方法
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.AttractionProducts
                .Include(p => p.Attraction)
                .Include(p => p.TicketType)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }
        // GET: Attractions/AttractionTicket/Create
        public IActionResult Create()
        {
            var attractions = _context.Attractions.ToList();
            ViewBag.AttractionList = new SelectList(attractions, "AttractionId", "Name");
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName");
            return View();
        }

        // POST: Attractions/AttractionTicket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttractionProduct product)
        {
            // 1. 移除不必要的模型驗證，避免導覽屬性（Navigation Property）為空導致驗證失敗
            ModelState.Remove("Attraction");
            ModelState.Remove("TicketType");

            if (ModelState.IsValid)
            {
                // 2. 自動補上建立時間
                product.CreatedAt = DateTime.Now;

                // 3. 處理 Status 字串標準化與 IsActive 的連動邏輯
                // 確保 Status 有值且為大寫，若沒填則給予預設值 "DRAFT"
                product.Status = product.Status?.ToUpper().Trim() ?? "DRAFT";

                // --- 自動連動邏輯：只有 ACTIVE 才上架，其餘狀態一律下架 ---
                if (product.Status == "ACTIVE")
                {
                    product.IsActive = 1; // 銷售中
                }
                else
                {
                    product.IsActive = 0; // 已下架 (適用於 DRAFT, INACTIVE, ARCHIVED)
                }

                try
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    // 加上操作成功的提示訊息（使用 TempData 搭配 SweetAlert 或 Toast）
                    TempData["SuccessMessage"] = $"票券「{product.Title}」已成功新增，狀態為 {product.Status}！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // 處理資料庫存檔時的例外
                    ModelState.AddModelError("", "存檔至資料庫時發生錯誤：" + ex.Message);
                }
            }

            // --- 若驗證失敗 (ModelState.IsValid == false) 或發生 Catch 異常 ---

            // 1. 輸出偵錯訊息
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine("驗證錯誤: " + error);
            }

            // 2. 重要！重新準備所有下拉選單資料，否則 View 重新渲染時會噴 NULL 錯誤
            // 景點清單 (過濾已軟刪除的景點)
            ViewBag.AttractionList = new SelectList(_context.Attractions.Where(a => !a.IsDeleted), "AttractionId", "Name", product.AttractionId);

            // 票種清單 (對應你的 TicketTypes 表)
            ViewBag.TicketTypeList = new SelectList(_context.TicketTypes.OrderBy(t => t.SortOrder), "TicketTypeCode", "TicketTypeName", product.TicketTypeCode);

            return View(product);
        }

        // --- 狀態切換功能 (IsActive) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, int isActive)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.IsActive = isActive;
            _context.Update(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isActive == 1 ? "票券已成功上架！" : "票券已下架。";
            return RedirectToAction(nameof(Index));
        }

        // --- 系統狀態變更 (Status) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var product = await _context.AttractionProducts.FindAsync(id);
            if (product == null) return NotFound();

            string cleanStatus = status.ToUpper().Trim();
            product.Status = cleanStatus;

            // --- 嚴謹連動邏輯開始 ---
            if (cleanStatus == "ACTIVE")
            {
                // 只有狀態為 ACTIVE 時，銷售狀態才設為 1 (銷售中)
                product.IsActive = 1;
            }
            else
            {
                // 只要不是 ACTIVE (包含 DRAFT, INACTIVE, ARCHIVED)，一律設為 0 (已下架)
                product.IsActive = 0;
            }
            // --- 嚴謹連動邏輯結束 ---

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                string msg = cleanStatus == "ACTIVE" ? "票券已正式發佈並開始銷售！" : "狀態已更新，銷售已停止。";
                TempData["SuccessMessage"] = msg;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "同步失敗：" + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
