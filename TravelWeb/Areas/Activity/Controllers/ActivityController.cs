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

        private readonly ActivityDbContext _dbContext;
        private readonly IPhotoService _photoService;

        public ActivityController(ActivityDbContext dbContext,IPhotoService photoService)
        {
            _dbContext = dbContext;
            _photoService = photoService;
        }


        //活動總覽
        [HttpGet("Overview")]
        public async Task<IActionResult> Index()
        {
            var acts = await _dbContext.Activities.Select(m => new ActivityEditViewModel
            {
                ActivityId = m.ActivityId,
                Title = m.Title,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Address = m.Address,
                Description = m.Description,
                OfficialLink = m.OfficialLink,
                UpdateAt = m.UpdateAt,
                RegionName = m.Regions.Select(r => r.RegionName).ToList(),
                TypeName = m.Types.Select(t => t.ActivityType).ToList()
            }).ToListAsync();

            return View(acts);
        }



        //新增活動
        [HttpGet("Create")]
        public IActionResult ActivityCreate() 
        {
            //Checkbox 填入資料用
            var Type = _dbContext.TagsActivityTypes.Select(m => m.ActivityType);
            var Region = _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName);
            ViewData["Type"] = Type;
            ViewData["Region"] = Region;

            return View("Create");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivityCreate(ActivityEditViewModel vm, List<IFormFile> images)
        {
            // 1. 如果驗證失敗，立即準備下拉選單資料並回傳
            if (!ModelState.IsValid)
            { 
                ViewData["Type"] = await _dbContext.TagsActivityTypes.Select(m => m.ActivityType).ToListAsync();
                ViewData["Region"] = await _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName).ToListAsync();
                return View("Create", vm);
            }

            try
            {
                // 2. 建立 Entity 實體
                var act = new Models.EFModel.Activity()
                {
                    Title = vm.Title,
                    StartTime = vm.StartTime,
                    EndTime = vm.EndTime,
                    Address = vm.Address,
                    OfficialLink = vm.OfficialLink,
                    Description = vm.Description,
                    UpdateAt = DateTime.Now,
                };

                // 3. 處理多對多關聯 - 類型
                if (vm.TypeName != null && vm.TypeName.Any())
                {
                    var selectedTypes = await _dbContext.TagsActivityTypes
                                            .Where(t => vm.TypeName.Contains(t.ActivityType))
                                            .ToListAsync();
                    foreach (var t in selectedTypes) act.Types.Add(t);
                }

                // 4. 處理多對多關聯 - 區域
                if (vm.RegionName != null && vm.RegionName.Any())
                {
                    var selectedRegions = await _dbContext.TagsRegions
                                              .Where(r => vm.RegionName.Contains(r.RegionName))
                                              .ToListAsync();
                    foreach (var r in selectedRegions) act.Regions.Add(r);
                }

                //照片存取到雲端，資料庫只存網址
                if (images !=null && images.Count > 0) 
                {
                    foreach (var image in images)
                    {
                        var result = await _photoService.AddPhotoAsync(image);

                        act.ActivityImages.Add(new ActivityImage
                        {
                            ImageUrl = result.SecureUrl.AbsoluteUri,
                            PublicId = result.PublicId,
                        });
                    }
                }

                // 5. 存檔
                _dbContext.Activities.Add(act);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 建議記錄錯誤訊息 ex.Message，方便除錯
                ModelState.AddModelError("", "資料庫存檔失敗：" + ex.Message);

                // 重新填入 ViewData 供頁面顯示
                ViewData["Type"] = await _dbContext.TagsActivityTypes.Select(m => m.ActivityType).ToListAsync();
                ViewData["Region"] = await _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName).ToListAsync();
                return View("Create", vm);
            }
        }



        //活動內容修改
        [HttpGet("Edit/{id}")]
        public IActionResult ActivityEdit(int id) 
        {
            //Checkbox 填入資料用
            var Type = _dbContext.TagsActivityTypes.Select(m => m.ActivityType);
            var Region = _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName);
            ViewData["Type"] = Type;
            ViewData["Region"] = Region;

            //撈取活動總表資料,並包裝成 ViewModel
            


            var act = _dbContext.Activities
                .Where(m => m.ActivityId == id)
                .Select(m => new ActivityEditViewModel 
                {
                    ActivityId = m.ActivityId,
                    Title = m.Title,
                    StartTime = m.StartTime,
                    EndTime = m.EndTime,
                    Address = m.Address,
                    Description = m.Description,
                    OfficialLink = m.OfficialLink,
                    RegionName = m.Regions.Select(r => r.RegionName).ToList(),
                    TypeName = m.Types.Select(t => t.ActivityType).ToList(),
                    ImgUrls = m.ActivityImages.Where(i => i.ActivityId == m.ActivityId).Select(u => u.ImageUrl).ToList(),
                }).FirstOrDefault();



            
            if (act == null)
            {
                return NotFound();
            }

            return View("Edit",act);
        }


        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivityEdit(int id, ActivityEditViewModel vm)
        {
            // 1. 安全檢查：確保網址 ID 與表單內容一致
            if (id != vm.ActivityId)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                var Type = _dbContext.TagsActivityTypes.Select(m => m.ActivityType);
                var Region = _dbContext.TagsRegions.Where(m => m.Uid == null).Select(m => m.RegionName);
                ViewData["Type"] = Type;
                ViewData["Region"] = Region;
                return View("Edit", vm);
            }

            var act = await _dbContext.Activities
                    .Include(a => a.Types)
                    .Include(a => a.Regions)
                    .FirstOrDefaultAsync(a => a.ActivityId == vm.ActivityId);

            if (act == null) return NotFound();

            // 2. 更新基本欄位 (解包 ViewModel)
            //act.ActivityId = vm.ActivityId;
            act.Title = vm.Title;
            act.StartTime = vm.StartTime;
            act.EndTime = vm.EndTime;
            act.OfficialLink = vm.OfficialLink;
            act.Address = vm.Address;
            act.Description = vm.Description;

            // 3. 處理標籤更新 (重要！)
            // 先清空舊有的關聯 (如果是多對多)
            act.Types.Clear();

            // 根據 ViewModel 的內容，去資料庫找出對應的標籤物件
            // 假設 vm.TypeName 現在是 List<string> 或單一字串
            if (vm.TypeName != null && vm.TypeName.Any()) 
            {
                var selectedTypes = await _dbContext.TagsActivityTypes
                .Where(t => vm.TypeName.Contains(t.ActivityType))
                .ToListAsync();

                foreach (var t in selectedTypes)
                {
                    act.Types.Add(t); // 加入新的關聯
                }
            }

            act.Regions.Clear();
            if (vm.RegionName != null && vm.RegionName.Any()) 
            {
                var selectedRegions = await _dbContext.TagsRegions
                .Where(r => vm.RegionName.Contains(r.RegionName))
                .ToListAsync();

                foreach (var r in selectedRegions)
                {
                    act.Regions.Add(r); // 加入新的關聯
                }
            }

            // 4. 儲存
            act.UpdateAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }






        //活動票卷設定
        [HttpGet("Ticket")]
        public async Task<IActionResult> TicketManage() 
        {
            var vm = await _dbContext.AcitivityTickets
                .Select(t => new ActivityTicketViewModel
                {
                    ProductCode = t.ProductCode,
                    ProductName = t.ProductName,
                    TicketCategoryName = t.TicketCategory!.CategoryName,
                    StartDate = t.StartDate,
                    ExpiryDate = t.ExpiryDate,
                    CurrentPrice = t.CurrentPrice,
                    Status = t.Status,
                }).ToListAsync();

            return View("Ticket",vm);
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
