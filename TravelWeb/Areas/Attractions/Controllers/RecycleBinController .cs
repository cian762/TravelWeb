using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;

namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class RecycleBinController : Controller
    {
        private readonly AttractionsContext _context;

        public RecycleBinController(AttractionsContext context)
        {
            _context = context;
        }

        // 顯示所有被軟刪除的景點與票券
        public async Task<IActionResult> Index()
        {
            // 被刪除的景點
            var deletedAttractions = await _context.Attractions
                .Include(a => a.Region)
                .Where(a => a.IsDeleted)
                .OrderByDescending(a => a.AttractionId)
                .ToListAsync();

            // 被刪除的票券
            var deletedTickets = await _context.AttractionProducts
                .Include(p => p.Attraction)
                .Include(p => p.TicketType)
                .Where(p => p.IsDeleted)
                .OrderByDescending(p => p.ProductId)
                .ToListAsync();

            ViewBag.DeletedAttractions = deletedAttractions;
            ViewBag.DeletedTickets = deletedTickets;

            return View();
        }

        // 恢復景點
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreAttraction(int id)
        {
            var attraction = await _context.Attractions.FindAsync(id);
            if (attraction == null) return NotFound();

            attraction.IsDeleted = false;
            // 恢復後設為審核中，讓管理員重新確認
            attraction.ApprovalStatus = 0;
            _context.Update(attraction);  // ← 加這行
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"景點「{attraction.Name}」已成功恢復，狀態已重設為審核中。";
            TempData["ActiveTab"] = "attractions";
            return RedirectToAction(nameof(Index));
        }

        // 恢復票券
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreTicket(int id)
        
        {
            var ticket = await _context.AttractionProducts.FindAsync(id);
            if (ticket == null) return NotFound();

            ticket.IsDeleted = false;
            // 恢復後設為草稿，避免直接上架
            ticket.Status = "DRAFT";
            ticket.IsActive = 0;
            _context.Update(ticket);  // ← 加這行
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"票券「{ticket.Title}」已成功恢復，狀態已重設為草稿。";
            TempData["ActiveTab"] = "tickets";
            return RedirectToAction(nameof(Index));
        }

     

      
    }
}
