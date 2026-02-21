using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Models;

namespace TravelWeb.Areas.Attractions.Controllers
{
    [Route("Attractions/[controller]/[action]")]
    public class TagsController : Controller
    {
        private readonly AttractionsContext _context;
        public TagsController(AttractionsContext context) => _context = context;

        // 給 Select2 搜尋用的 API
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var tags = await _context.Tags
                .Where(t => t.TagName.Contains(term))
                .Select(t => new { id = t.TagId, text = t.TagName })
                .ToListAsync();
            return Json(tags);
        }

     
    }
}
