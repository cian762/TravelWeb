using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;

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
            var result = _dbContext.Activities.FirstOrDefault(m=> m.ActivityId == id);
            if (result == null)
            {
                return NotFound();
            }
            return View("Edit",result);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult ActivityEdit(int id, TravelWeb.Areas.Activity.Models.EFModel.Activity activity)
        {
            // 1. 安全檢查：確保網址 ID 與表單內容一致
            if (id != activity.ActivityId)
            {
                return BadRequest();
            }

            
            if (ModelState.IsValid)
            {
                try
                {
                    activity.UpdateAt = DateTime.Now;
                    _dbContext.Update(activity);
                    _dbContext.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "更新失敗：" + ex.Message);
                }
            }
            return View("Edit", activity);
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
