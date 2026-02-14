using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Controllers
{
    [Area("Activity")]
    [Route("Act")]
    public class ActivityController : Controller
    {

        private readonly ActivityDbContext _dbContext;

        public ActivityController(ActivityDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //活動總覽
        [HttpGet("Overview")]
        public IActionResult Index()
        {
             var result = _dbContext.Activities;
            return View(result);
        }

        //活動內容設定

        [HttpGet("Edit/{id}")]
        public IActionResult ActivityEdit(int id) 
        {
            //下拉式選單填入資料
            var Type = _dbContext.TagsActivityTypes.Select(m => m.ActivityType);
            var Region = _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName);
            ViewData["Type"] = Type;
            ViewData["Region"] = Region;


            //撈取活動總表資料,並包裝成 ViewModel
            var act = _dbContext.Activities
                .Where(m => m.ActivityId == id)
                .Select(m => new ActivityEditViewModel 
                {
                    ActivityInfo = m,
                    RegoinName = m.Regions.Select(r => r.RegionName).ToList(),
                    TypeName = m.Types.Select(t => t.ActivityType).ToList()
                }).FirstOrDefault();
            
            if (act == null)
            {
                return NotFound();
            }

            return View("Edit",act);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> ActivityEdit(int id, ActivityEditViewModel vm)
        {
            // 1. 安全檢查：確保網址 ID 與表單內容一致
            if (id != vm.ActivityInfo.ActivityId)
            {
                return BadRequest();
            }

            
            if (ModelState.IsValid)
            {
                var act = await _dbContext.Activities
            .Include(a => a.Types)
            .Include(a => a.Regions)
            .FirstOrDefaultAsync(a => a.ActivityId == vm.ActivityInfo.ActivityId);

                if (act == null) return NotFound();

                // 2. 更新基本欄位 (解包 ViewModel)
                act.Title = vm.ActivityInfo.Title;
                act.StartTime = vm.ActivityInfo.StartTime;
                act.EndTime = vm.ActivityInfo.EndTime;
                act.OfficialLink = vm.ActivityInfo.OfficialLink;
                act.Address = vm.ActivityInfo.Address;
                act.Description = vm.ActivityInfo.Description;

                // 3. 處理標籤更新 (重要！)
                // 先清空舊有的關聯 (如果是多對多)
                act.Types.Clear();

                // 根據 ViewModel 的內容，去資料庫找出對應的標籤物件
                // 假設 vm.TypeName 現在是 List<string> 或單一字串
                var selectedTypes = await _dbContext.TagsActivityTypes
                            .Where(t => vm.TypeName.Contains(t.ActivityType))
                            .ToListAsync();

                foreach (var t in selectedTypes)
                {
                    act.Types.Add(t); // 加入新的關聯
                }

                act.Regions.Clear();

                var selectedRegions = await _dbContext.TagsRegions
            .Where(r => vm.RegoinName.Contains(r.RegionName))
            .ToListAsync();


                foreach (var r in selectedRegions)
                {
                    act.Regions.Add(r); // 加入新的關聯
                }

                // 4. 儲存
                act.UpdateAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View("Edit", vm);
        }

        //活動票卷設定
        [HttpGet("Ticket")]
        public IActionResult TicketManage() 
        {
            return View();
        }

        [HttpPost("TicketEdit")]
        public IActionResult TicketManage(int a) 
        {
            return RedirectToAction(nameof(TicketManage));
        }

        // 活動排程設定
        [HttpGet("Schedule")]
        public IActionResult ActivitySetUp() 
        {
            return View();
        }

        [HttpPost("Schedule")]
        public IActionResult ActivitySetUp(int a) 
        {
            return RedirectToAction(nameof(ActivitySetUp));
        }

        //活動熱度分析


    }
}
