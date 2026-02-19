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
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }
        // GET: Attractions/AttractionTicket/Create
        public IActionResult Create()
        {
            var attractions = _context.Attractions.ToList();
            ViewBag.AttractionList = new SelectList(attractions, "AttractionId", "Name");
            return View();
        }

        // POST: Attractions/AttractionTicket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttractionProduct product)
        {
            // 移除不必要的驗證項（如果導覽屬性沒改 nullable，這行可以手動移除該項目的驗證）
            ModelState.Remove("Attraction");

            if (ModelState.IsValid)
            {
                // 1. 自動補上建立時間
                product.CreatedAt = DateTime.Now;

                // 2. 處理 Status：如果畫面上沒填，就補上資料庫的預設值 'DRAFT'
                if (string.IsNullOrEmpty(product.Status))
                {
                    product.Status = "DRAFT";
                }

                // 3. 處理 RegionId：如果你的系統一定要有區域 ID
                if (product.RegionId == null || product.RegionId == 0)
                {
                    product.RegionId = 1; // 根據你的資料庫情況給予一個預設值
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                // 加上操作成功的提示訊息
                TempData["SuccessMessage"] = $"票券「{product.Title}」已成功新增！";

                return RedirectToAction(nameof(Index));
            }

            // --- 驗證失敗時的除錯與回傳 ---
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine("驗證錯誤: " + error);
            }

            ViewBag.AttractionList = new SelectList(_context.Attractions, "AttractionId", "Name", product.AttractionId);
            return View(product);
        }


        [HttpPost]
        public IActionResult ToggleActive(int id, int isActive)
        {
            var product = _context.AttractionProducts.Find(id);
            if (product != null)
            {
                product.IsActive = isActive;
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"票券狀態已成功更新！";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
