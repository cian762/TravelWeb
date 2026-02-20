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

        // 當系統找不到標籤時，按 Enter 呼叫這個 API
        [HttpPost]
        public async Task<IActionResult> QuickCreate(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return BadRequest();

            // 檢查是否已存在 (防呆)
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
            if (existingTag != null) return Json(new { id = existingTag.TagId, text = existingTag.TagName });

            var newTag = new Tag { TagName = tagName };
            _context.Tags.Add(newTag);
            await _context.SaveChangesAsync();

            return Json(new { id = newTag.TagId, text = newTag.TagName });
        }
    }
}
