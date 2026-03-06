using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Service.IActivityServices;

namespace TravelWeb.Areas.Activity.Controllers
{
    [Area("Activity")]
    [Route("Act")]
    public class ActivityController : Controller
    {

        private readonly IActivityInfoService _activityInfoService;

        private readonly IActivityTicketService _activityTicketService;

        private List<string> TypeNameCollection { get; set; }

        private List<string> RegionNameCollection { get; set; }

        public ActivityController(IActivityInfoService activityInfoService,IActivityTicketService activityTicketService)
        {
            _activityInfoService = activityInfoService;
            _activityTicketService = activityTicketService;

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
        //[HttpGet("Ticket")]
        //public async Task<IActionResult> TicketCreate()
        //{
        //    var ActivityInfo = await _dbContext.Activities.Select(a => new
        //    {
        //        ActivityId = a.ActivityId,
        //        ActivityName = a.Title
        //    }).ToListAsync();


        //    ViewData["TicketCategory"] = await _dbContext.TicketCategories.Select(c => c.CategoryName).ToListAsync();
        //    ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
        //    ViewData["ActivityInfo"] = ActivityInfo;
        //    return View();
        //}

        //AJAX 拉商品資料用 Action
        [HttpGet("GetProductInfo")]
        public async Task<IActionResult> GetProductsByActivity(int activityId)
        {
            var products = await _dbContext.AcitivityTickets
                .Where(a => a.ActivityTicketDetail!.ActivityId == activityId)
                .Select(p => new
                {
                    name = p.ProductName,
                    category = p.TicketCategory!.CategoryName,
                    price = p.CurrentPrice,
                    status = p.Status
                }).ToListAsync();

            return Json(products);
        }


        // AJAX 拉活動起始/終止用 Action
        [HttpGet("GetActivityInfo")]
        public async Task<IActionResult> GetTimeByActivity(int activityId)
        {
            var result = await _dbContext.Activities
                .Where(a => a.ActivityId == activityId)
                .Select(a => new
                {
                    startTime = a.StartTime,
                    endTime = a.EndTime
                }).FirstOrDefaultAsync();

            return Json(result);
        }


        ////活動票劵新增 POST
        //[HttpPost("Ticket")]
        //public async Task<IActionResult> TicketCreate(ActivityTicketViewModel ticket)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(ticket);
        //    }

        //    var ActivityInfo = await _dbContext.Activities.Select(a => new
        //    {
        //        ActivityId = a.ActivityId,
        //        ActivityName = a.Title
        //    }).ToListAsync();

        //    ViewData["TicketCategory"] = await _dbContext.TicketCategories.Select(c => c.CategoryName).ToListAsync();
        //    ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
        //    ViewData["ActivityInfo"] = ActivityInfo;



        //    // 新產品 ProductCode 定義
        //    var lastProductCode = await _dbContext.AcitivityTickets
        //        .OrderByDescending(p => p.ProductCode)
        //        .Select(p => p.ProductCode)
        //        .FirstOrDefaultAsync();


        //    string newProductCode;

        //    if (string.IsNullOrEmpty(lastProductCode))
        //    {
        //        newProductCode = "ACT-0001";
        //    }
        //    else
        //    {
        //        if (int.TryParse(lastProductCode.Substring(4), out int lastNumber))
        //        {
        //            int nextNumber = lastNumber + 1;
        //            newProductCode = $"ACT-{nextNumber:D4}";
        //        }
        //        else
        //        {
        //            newProductCode = "ACT-0001";
        //        }
        //    }

        //    // 將 VM mapping 到 EF model

        //    var activtiyName = await _dbContext.Activities.Where(a => a.ActivityId == ticket.AcivityId).Select(p => p.Title).FirstOrDefaultAsync();

        //    var product = new AcitivityTicket()
        //    {
        //        ProductCode = newProductCode,
        //        ProductName = $"{activtiyName} | {ticket.TicketCategoryName}",
        //        TicketCategoryId = await _dbContext.TicketCategories.Where(t => t.CategoryName == ticket.TicketCategoryName).Select(p => p.TicketCategoryId).FirstOrDefaultAsync(),
        //        StartDate = ticket.StartDate,
        //        ExpiryDate = ticket.ExpiryDate,
        //        CurrentPrice = ticket.CurrentPrice,
        //        Status = ticket.Status
        //    };



        //    var productDetail = new ActivityTicketDetail()
        //    {
        //        ProductCode = newProductCode,
        //        ActivityId = await _dbContext.Activities.Where(a => a.ActivityId == ticket.AcivityId).Select(p => p.ActivityId).FirstOrDefaultAsync(),
        //        ProdcutDescription = ticket.ProdcutDescription,
        //        TermsOfService = ticket.TermsOfService
        //    };


        //    //寫回資料庫
        //    _dbContext.AcitivityTickets.Add(product);
        //    _dbContext.ActivityTicketDetails.Add(productDetail);
        //    await _dbContext.SaveChangesAsync();

        //    return RedirectToAction(nameof(TicketCreate));
        //}





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


        ////活動票券修改 POST
        //[HttpPost("Ticket/{ProductCode}")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> TicketEdit(string ProductCode, ActivityTicketViewModel ticket)
        //{
        //    if (ProductCode != ticket.ProductCode)
        //    {
        //        return BadRequest();
        //    }


        //    if (!ModelState.IsValid)
        //    {
        //        var ActivityInfo = await _dbContext.Activities.Select(a => new
        //        {
        //            ActivityId = a.ActivityId,
        //            ActivityName = a.Title
        //        }).ToListAsync();

        //        ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
        //        ViewData["ActivityInfo"] = ActivityInfo;

        //        return View("TicketEdit", ticket);
        //    }



        //    var OriginTicket = await _dbContext.AcitivityTickets
        //        .FirstOrDefaultAsync(t => t.ProductCode == ticket.ProductCode);


        //    OriginTicket!.CurrentPrice = ticket.CurrentPrice;
        //    OriginTicket!.Status = ticket.Status;
        //    OriginTicket!.StartDate = ticket.StartDate;
        //    OriginTicket!.ExpiryDate = ticket.ExpiryDate;



        //    var OriginTicketDetail = await _dbContext.ActivityTicketDetails
        //        .FirstOrDefaultAsync(d => d.ProductCode == ticket.ProductCode);

        //    OriginTicketDetail!.ProdcutDescription = ticket.ProdcutDescription;
        //    OriginTicketDetail!.TermsOfService = ticket.TermsOfService;


        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        ModelState.AddModelError("", "更新失敗: " + ex.Message);

        //        var ActivityInfo = await _dbContext.Activities.Select(a => new
        //        {
        //            ActivityId = a.ActivityId,
        //            ActivityName = a.Title
        //        }).ToListAsync();

        //        ViewData["Status"] = new List<string>() { "預告中", "販售中", "已售完", "已下架" };
        //        ViewData["ActivityInfo"] = ActivityInfo;

        //        return View("TicketEdit", ticket);

        //    }
        //    return RedirectToAction(nameof(TicketManage));
        //}







        //// 活動排程設定
        //[HttpGet("Schedule")]
        //public IActionResult ActivitySetUp() 
        //{
        //    return View();
        //}

        //[HttpPost("Schedule")]
        //public IActionResult ActivitySetUp(int a) 
        //{
        //    return RedirectToAction(nameof(ActivitySetUp));
        //}

        //活動熱度分析


    }
}
