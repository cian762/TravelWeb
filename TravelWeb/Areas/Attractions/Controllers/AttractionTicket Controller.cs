using Microsoft.AspNetCore.Mvc;
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
            // 抓取所有景點，準備給下拉選單使用
            // 注意：這裡的 Select 欄位名稱要對應你的 Attraction Model
            var attractions = _context.Attractions
                .Select(a => new { a.AttractionId, a.Name })
                .ToList();

            ViewBag.AttractionList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(attractions, "AttractionId", "Name");

            return View();
        }

        // POST: Attractions/AttractionTicket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttractionProduct product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now; // 自動填入建立時間
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 如果失敗，重新準備下拉選單資料
            var attractions = _context.Attractions.Select(a => new { a.AttractionId, a.Name }).ToList();
            ViewBag.AttractionList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(attractions, "AttractionId", "Name");
            return View(product);
        }
    }
}
