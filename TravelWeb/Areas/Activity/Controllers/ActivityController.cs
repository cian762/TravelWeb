using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Service.IActivityServices;
using TravelWeb.Filters;

namespace TravelWeb.Areas.Activity.Controllers
{
    [AdminAuthorize]
    [Area("Activity")]
    [Route("Act")]
    public class ActivityController : Controller
    {
        private readonly ActivityDbContext _dbcontext;

        private readonly IActivityInfoService _activityInfoService;

        private readonly IActivityTicketService _activityTicketService;

        private List<string> TypeNameCollection { get; set; }

        private List<string> RegionNameCollection { get; set; }

        public ActivityController(IActivityInfoService activityInfoService,IActivityTicketService activityTicketService,ActivityDbContext dbcontext)
        {
            _activityInfoService = activityInfoService;
            _activityTicketService = activityTicketService;
            _dbcontext = dbcontext;

            TypeNameCollection = _activityInfoService.ProvideTypeTag();
            RegionNameCollection = _activityInfoService.ProvideRegionTag();
        }


        //活動總覽 GET
        [HttpGet("Activities")]
        public async Task<IActionResult> Index()
        {
            var data = await _activityInfoService.GetAllActInfoAsync();

            return View(data);
        }


        //新增活動 GET
        [HttpGet("Activity")]
        public IActionResult ActivityCreate()
        {
            //Checkbox 填入資料用
            ViewData["Type"] = TypeNameCollection;
            ViewData["Region"] = RegionNameCollection;

            return View("Create");
        }

        //新增活動 POST
        [HttpPost("Activity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivityCreate(ActivityInfoViewModel vm, List<IFormFile> images)
        {
            ViewData["Type"] = TypeNameCollection;
            ViewData["Region"] = RegionNameCollection;
            
            // 1. 如果驗證失敗，立即準備下拉選單資料並回傳
            if (!ModelState.IsValid)
            {
                return View("Create", vm);
            }

            try
            {
                await _activityInfoService.CreateActInfoAsync(vm, images);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //// 建議記錄錯誤訊息 ex.Message，方便除錯
                ModelState.AddModelError("", "資料庫存檔失敗：" + ex.Message);

                //// 重新填入 ViewData 供頁面顯示
                return View("Create", vm);
            }
        }



        //活動內容修改 GET
        [HttpGet("Activity/{id}")]
        public async Task<IActionResult> ActivityEdit(int id)
        {
            //Checkbox 填入資料用

            ViewData["Type"] = TypeNameCollection;
            ViewData["Region"] = RegionNameCollection;

            //撈取活動總表資料,並包裝成 ViewModel

            var act = await _activityInfoService.GetActInfoByIdAsync(id);

            if (act == null)
            {
                return NotFound();
            }

            return View("Edit", act);
        }

        //活動內容修改 POST
        [HttpPost("Activity/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivityEdit(int id, ActivityInfoViewModel vm, List<IFormFile> images, List<string> DeletedUrls)
        {
            ViewData["Type"] = TypeNameCollection;
            ViewData["Region"] = RegionNameCollection;

            // 1. 安全檢查：確保網址 ID 與表單內容一致
            if (id != vm.ActivityId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", vm);
            }

            try
            {
                await _activityInfoService.EditActInfoAsync(vm, images, DeletedUrls);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //// 建議記錄錯誤訊息 ex.Message，方便除錯
                ModelState.AddModelError("", "資料庫存檔失敗：" + ex.Message);

                //// 重新填入 ViewData 供頁面顯示
                return View("Edit", vm);
            }
        }


        //活動軟刪除
        [HttpPost("Activity/Delete/{id}")]
        public async Task<IActionResult> ActivitySoftDelete(int id) 
        {
            await _activityInfoService.DeleteActInfoAsync(id);
            return RedirectToAction(nameof(Index));
        }



        //活動票卷總覽 GET
        [HttpGet("Tickets")]
        public async Task<IActionResult> TicketManage()
        {
            var vm = await _activityTicketService.GetAllActTicketsAsync();
            return View("Ticket", vm);
        }




        ////活動票劵新增 GET
        [HttpGet("Ticket")]
        public async Task<IActionResult> TicketCreate()
        {

            var act = await _activityInfoService.GetAllActInfoAsync();

            var ActivityInfo = act.Select(a =>
            new
            {
                ActivityId = a.ActivityId,
                ActivityName = a.Title
            })
            .ToList();

            ViewData["TicketCategory"] = await _activityTicketService.TakeTicketCategoryNames();
            ViewData["ActivityInfo"] = ActivityInfo;
            ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
            return View();
        }



        //AJAX 拉商品資料用 Action
        [HttpGet("GetProductInfo")]
        public async Task<IActionResult> GetProductsByActivity(int activityId)
        {
            var ticket = await _activityTicketService.GetActTicketByActIdAsync(activityId);
            
            var products = ticket.Select(p => new
            {
                name = p.ProductName,
                category = p.TicketCategory!.CategoryName,
                price = p.CurrentPrice,
                status = p.Status
            }).ToList();

            return Json(products);
        }


        // AJAX 拉活動起始/終止用 Action
        [HttpGet("GetActivityInfo")]
        public async Task<IActionResult> GetTimeByActivity(int activityId)
        {
            var result = await _activityInfoService.GetActInfoByIdAsync(activityId);

            return Json( new { startTime = result?.StartTime, endTime = result?.EndTime } );
        }


        //活動票劵新增 POST
        [HttpPost("Ticket")]
        public async Task<IActionResult> TicketCreate(ActivityTicketViewModel ticket)
        {
            var act = await _activityInfoService.GetAllActInfoAsync();

            var ActivityInfo = act.Select(a =>
            new
            {
                ActivityId = a.ActivityId,
                ActivityName = a.Title
            })
            .ToList();

            ViewData["TicketCategory"] = await _activityTicketService.TakeTicketCategoryNames();
            ViewData["ActivityInfo"] = ActivityInfo;
            ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            await _activityTicketService.CreateActTicketAsync(ticket);

            return RedirectToAction(nameof(TicketCreate));
        }





        //活動票券修改 GET
        [HttpGet("Ticket/{ProductCode}")]
        public async Task<IActionResult> TicketEdit(string ProductCode)
        {
            var act = await _activityInfoService.GetAllActInfoAsync();

            var ActivityInfo = act.Select(a => 
            new {
                ActivityId = a.ActivityId,
                ActivityName = a.Title
            })
            .ToList();

            ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
            ViewData["ActivityInfo"] = ActivityInfo;

            var product = await _activityTicketService.GetActTicketByProductCodeAsync(ProductCode);

            return View("TicketEdit", product);
        }


        //活動票券修改 POST
        [HttpPost("Ticket/{ProductCode}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TicketEdit(string ProductCode, ActivityTicketViewModel ticket)
        {
            if (ProductCode != ticket.ProductCode)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var act = await _activityInfoService.GetAllActInfoAsync();

                var ActivityInfo = act.Select(a =>
                new {
                    ActivityId = a.ActivityId,
                    ActivityName = a.Title
                })
                .ToList();

                ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
                ViewData["ActivityInfo"] = ActivityInfo;

                return View("TicketEdit", ticket);
            }

            await _activityTicketService.EditActTicketAsync(ProductCode, ticket);

            return RedirectToAction(nameof(TicketManage));
        }







        // 活動排程設定
        [HttpGet("Schedule")]
        public IActionResult ActivitySetUp()
        {
            var plan = _dbcontext.ActivityPublishStatuses
                .Include(a => a.Activity)
                .Select(a => new ActivityScheduleViewModel
                {
                    Title = a.Activity.Title,
                    StartTime = a.Activity.StartTime,
                    EndTime = a.Activity.EndTime,
                    PublishTime = a.PublishTime,
                    UnPublishTime = a.UnPublishTime,
                    Status = a.Status
                }).ToList();

            return View("Schedule", plan);
        }



        [HttpGet("GetScheduleInfo")]
        // AJAX 拉取資料放到 FullCalendar
        public IActionResult GetScheduleInfo() 
        {
            var plan = _dbcontext.ActivityPublishStatuses
                   .Include(a => a.Activity)
                   .Select(a => new 
                   {
                       title = a.Activity.Title,
                       start = a.PublishTime,
                       end = a.UnPublishTime,
                       allDay = true,
                   }).ToList();
            return Json(plan);
        }



        //[HttpPost("Schedule")]
        //public IActionResult ActivitySetUp(int a) 
        //{
        //    return RedirectToAction(nameof(ActivitySetUp));
        //}



        //活動熱度分析


    }
}
