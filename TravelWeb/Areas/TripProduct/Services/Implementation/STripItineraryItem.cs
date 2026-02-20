using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Collections.Generic;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;
using static System.Net.Mime.MediaTypeNames;


namespace TravelWeb.Areas.TripProduct.Services.Implementation
{
    
    public class STripItineraryItem : ITripItineraryItem
    {
        private readonly TripDbContext _context;
        public STripItineraryItem(TripDbContext context) 
        {
         _context = context;
        }
        //抓取已排行程細項資料的方法
        public async Task<IEnumerable<ViewModelTripItineraryItems>> IGetAny(int tripProductId)
        {
            var dataList = await _context.TripItineraryItems
           .Include(t => t.Attraction)
           .Include(t => t.Activity)
           .Include(t => t.Resource)
           .Where(t => t.TripProductId == tripProductId)
           .OrderBy(t => t.DayNumber)
           .ThenBy(t => t.SortOrder)
           .ToListAsync(); // 👈 先轉成 List 才能分段執行並加斷點

            var resultList = new List<ViewModelTripItineraryItems>();

            // 2. 開始分段處理，這裡每一行都可以加斷點
            foreach (var s in dataList)
            {
                var vm = new ViewModelTripItineraryItems
                {
                    DayNumber = s.DayNumber,
                    SortOrder = s.SortOrder,
                    TripProductId = tripProductId,
                    CustomText = s.CustomText
                };

                // --- 處理名稱判斷 ---
                if (s.Attraction != null)
                {
                    vm.ResourceName = s.Attraction.Name; // 📍 斷點 1：景點名稱
                }
                else if (s.Activity != null)
                {
                    vm.ResourceName = s.Activity.Title; // 📍 斷點 2：活動名稱
                }
                else if (s.Resource != null)
                {
                    vm.ResourceName = s.Resource.ResourceName; // 📍 斷點 3：資源名稱
                }

                // --- 處理圖片路徑抓取 ---
                if (s.AttractionId != null)
                {
                    // 📍 斷點 4：去抓景點圖
                    vm.ImagePath = _context.Set<TravelWeb.Areas.Attractions.Models.Image>()
                        .Where(img => img.AttractionId == s.AttractionId)
                        .Select(img => img.ImagePath)
                        .FirstOrDefault() ?? "";
                }
                else if (s.ActivityId != null)
                {
                    // 📍 斷點 5：去抓活動圖
                    vm.ImagePath = _context.Set<TravelWeb.Areas.Activity.Models.EFModel.ActivityImage>()
                        .Where(img => img.ActivityId == s.ActivityId)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault() ?? "";
                }
                else if (s.Resource != null)
                {
                    // 📍 斷點 6：去抓資源圖
                    vm.ImagePath = s.Resource.ResourcesImages.Select(i => i.MainImage).FirstOrDefault() ?? "";
                }

                resultList.Add(vm);
            }

            return resultList;

        }
        //新增行程細項的方法
        public async Task<bool> ICreate(ViewModelTripItineraryItems items)
        {
            string targetName = "";
            string targetDescription = "";
            string targetImage = "";

            // 1. 抓取原始資料與描述邏輯
            if (items.ActivityId.HasValue)
            {
                // 💡 活動：從主表抓描述
                var act = await _context.Activities.FindAsync(items.ActivityId);
                targetName = act?.Title ?? "未命名活動";
                targetDescription = act?.Description ?? items.CustomText ?? ""; // 優先用活動描述，沒有才用手打的
                targetImage = await _context.Set<TravelWeb.Areas.Activity.Models.EFModel.ActivityImage>()
                    .Where(img => img.ActivityId == items.ActivityId)
                    .Select(img => img.ImageUrl).FirstOrDefaultAsync() ?? "";
            }
            else if (items.AttractionId.HasValue)
            {
                // 💡 景點：景點沒描述，直接用你手打的 CustomText
                var att = await _context.Attractions.FindAsync(items.AttractionId);
                targetName = att?.Name ?? "未命名景點";
                targetDescription = items.CustomText ?? "";
                targetImage = await _context.Set<TravelWeb.Areas.Attractions.Models.Image>()
                    .Where(img => img.AttractionId == items.AttractionId)
                    .Select(img => img.ImagePath).FirstOrDefaultAsync() ?? "";
            }
            else if (items.ResourceId.HasValue)
            {
                // 💡 自訂資源：直接沿用你輸入的內容
                targetDescription = items.CustomText ?? "";
            }

            // 2. 存入 Resources 表 (正式轉為資源)
            var newResource = new Resource
            {
                ResourceName = targetName,
                DefaultDescription = targetDescription, // 這裡存入最終確定的描述
                ShortDescription = ""
            };
            _context.Resources.Add(newResource);
            await _context.SaveChangesAsync();

            // 3. 存入 ResourcesImage 表
            if (!string.IsNullOrEmpty(targetImage))
            {
                var newResImg = new ResourcesImage
                {
                    ResourceId = newResource.ResourceId,
                    MainImage = targetImage
                };
                _context.ResourcesImages.Add(newResImg);
            }

            // 4. 建立行程細項
            var n = new TripItineraryItem
            {
                TripProductId = items.TripProductId,
                DayNumber = items.DayNumber,
                SortOrder = items.SortOrder,
                AttractionId = items.AttractionId,
                ActivityId = items.ActivityId,
                ResourceId = newResource.ResourceId, // 連結新資源 ID
                CustomText = null // 💡 描述已經存進 Resources 了，這裡維持乾淨
            };

            _context.TripItineraryItems.Add(n);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<bool> IDelete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IUpdate(ViewModelTripItineraryItems items)
        {
            throw new NotImplementedException();
        }
        //準備新增細項頁面所需的下拉選單與基礎資料
        public async Task<ViewModelTripItineraryItems> PrepareViewModel(int tripProductId)
        {
            var vm = new ViewModelTripItineraryItems();
            vm.TripProductId = tripProductId;
            vm.AttractionList = await _context.Attractions.Select(s=>new SelectListItem {
             Value=s.AttractionId.ToString(),
             Text = s.Name
            }).ToListAsync();
            vm.ActivityList=await _context.Activities.Select(s=>new SelectListItem { 
             Value =s.ActivityId.ToString(),
             Text = s.Title
            }).ToListAsync();
            var product = await _context.TripProducts.FindAsync(tripProductId);
            vm.MaxDays = product?.DurationDays ?? 1;
            return vm;
        }
        //行程細項抓圖的方法
        public async Task<ResourceDetailDto> GetResourceDetailAsync(string type, int id)
        {
            var result = new ResourceDetailDto();

            if (type == "attraction")
            {
                result.Images = await _context.Set<TravelWeb.Areas.Attractions.Models.Image>()
                    .Where(i => i.AttractionId == id && i.ImagePath != null) // 💡 確保路徑不為空
                    .Select(i => i.ImagePath!) // 💡 使用 ! 告訴編譯器這裡一定有值
                    .ToListAsync();
            }
            else if (type == "activity")
            {
                var act = await _context.Activities.FindAsync(id);
                result.Description = act?.Description ?? "";

                result.Images = await _context.Set<TravelWeb.Areas.Activity.Models.EFModel.ActivityImage>()
                    .Where(i => i.ActivityId == id && i.ImageUrl != null) // 💡 確保 URL 不為空
                    .Select(i => i.ImageUrl!)
                    .ToListAsync();
            }
            return result;
        }

        // ... 你的 IGetAny 和 ICreate 方法 ...
    }

    // 💡 寫在同一個 namespace 下，不用另外開檔
    public class ResourceDetailDto
    {
        public string Description { get; set; } = "";
        public List<string> Images { get; set; } = new List<string>();
    }

}
