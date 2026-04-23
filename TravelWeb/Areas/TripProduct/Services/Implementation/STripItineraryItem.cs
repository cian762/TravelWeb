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
            // 1. 抓取資料，並 Include 關聯與圖片
            var dataList = await _context.TripItineraryItems
                .Include(t => t.Attraction)
                .Include(t => t.Activity)
                .Include(t => t.Resource)
                    .ThenInclude(r => r!.ResourcesImages) // 💡 關鍵：抓取自定義資源的多張圖
                .Where(t => t.TripProductId == tripProductId)
                .OrderBy(t => t.DayNumber)
                .ThenBy(t => t.SortOrder)
                .ToListAsync();

            // 2. 轉換為 ViewModel
            var resultList = dataList.Select(s => new ViewModelTripItineraryItems
            {
                ItineraryItemId = s.ItineraryItemId,
                TripProductId = s.TripProductId,
                DayNumber = s.DayNumber,
                SortOrder = s.SortOrder,
                AttractionId = s.AttractionId,
                ActivityId = s.ActivityId,
                ResourceId = s.ResourceId,

                // 💡 名稱邏輯
                ResourceName = s.Resource?.ResourceName ??
                               (s.Attraction != null ? s.Attraction.Name :
                                s.Activity != null ? s.Activity.Title : "未命名項目"),

                // 💡 描述邏輯：這裡存你打的「交通時間、多久」等備註
                CustomText = s.CustomText ?? "",

                // 💡 處理多圖路徑：將 ResourcesImages 集合轉為字串清單
                AllImagePaths = s.Resource?.ResourcesImages
                 .Select(img => img.MainImage) // 或是 ImagePath，視你的實體欄位而定
                 .Where(path => path != null)
                 .Select(path => path!)
                 .ToList() ?? new List<string>()
            }).ToList();

            return resultList;

        }
        //新增行程細項的方法
        public async Task<bool> ICreate(ViewModelTripItineraryItems items)
        {
            string targetName = "";
            string resourceMainDescription = ""; // 用於存入 Resources 表的描述
            List<string> imagePaths = new List<string>();

            // --- A. 處理「原本就有的圖片」與名稱 ---

            if (items.ActivityId.HasValue)
            {
                // 1. 處理活動：抓取標題與原始描述
                var act = await _context.Activities.FindAsync(items.ActivityId);
                targetName = act?.Title ?? "未命名活動";

                // 優先取用畫面上傳回來的描述，若無則用資料庫原始描述
                resourceMainDescription = !string.IsNullOrEmpty(items.ResourceDescription)
                                          ? items.ResourceDescription
                                          : (act?.Description ?? "");

                // 2. 抓取活動原本的所有圖片路徑
                var actImages = await _context.ActivityImages
                  .Where(i => i.ActivityId == items.ActivityId && i.ImageUrl != null)
                  .Select(i => i.ImageUrl!)
                  .ToListAsync();
            }
            else if (items.AttractionId.HasValue)
            {
                // 1. 處理景點：抓取名稱 (景點確定沒有原始描述)
                var att = await _context.Attractions.FindAsync(items.AttractionId);
                targetName = att?.Name ?? "未命名景點";

                // 景點雖無原始描述，但仍保留畫面上手打的內容
                resourceMainDescription = items.ResourceDescription ?? "";

                // 2. 抓取景點原本的所有圖片路徑
                var attImages = await _context.Images
               .Where(img => img.AttractionId == items.AttractionId && img.ImagePath != null)
               .Select(img => img.ImagePath!)
               .ToListAsync();
            }
            else
            {
                // 1. 自定義資源情形
                targetName = items.ResourceName ?? "自定義行程";
                resourceMainDescription = items.ResourceDescription ?? "";
            }

            // --- B. 處理「新上傳的圖片」 (不論類型，有選檔案就處理) ---

            if (items.FileImages != null && items.FileImages.Count > 0)
            {
                foreach (var file in items.FileImages)
                {
                    if (file.Length > 0)
                    {
                        // 產生不重複檔名並存入 wwwroot/PImages
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PImages", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        // 將新檔案路徑加進 list
                        imagePaths.Add("/PImages/" + fileName);
                    }
                }
            }

            // --- C. 寫入資料庫 ---

            // 1. 建立資源主表 (Resource)
            var newResource = new Resource
            {
                ResourceName = targetName,
                DefaultDescription = resourceMainDescription,
                ShortDescription = ""
            };
            _context.Resources.Add(newResource);
            await _context.SaveChangesAsync(); // 先存以取得 ResourceId

            // 2. 建立資源圖片關聯 (ResourcesImage)
            if (imagePaths.Any())
            {
                foreach (var path in imagePaths)
                {
                    _context.ResourcesImages.Add(new ResourcesImage
                    {
                        ResourceId = newResource.ResourceId,
                        MainImage = path // 存入統一的圖片路徑
                    });
                }
                await _context.SaveChangesAsync();
            }

            // 3. 建立行程細項表 (TripItineraryItem)
            var n = new TripItineraryItem
            {
                TripProductId = items.TripProductId,
                DayNumber = items.DayNumber,
                SortOrder = items.SortOrder,
                AttractionId = items.AttractionId,
                ActivityId = items.ActivityId,
                ResourceId = newResource.ResourceId, // 關聯剛剛建立的 Resource
                CustomText = items.CustomText     // 存入行程私密小備註
            };

            _context.TripItineraryItems.Add(n);
            return await _context.SaveChangesAsync() > 0;
        }
        //刪除行程細項的方法
        public async Task<bool> IDelete(int id)
        {
            // 1. 抓取資料，使用 Include 確保關聯物件被載入
            var item = await _context.TripItineraryItems
                .Include(i => i.Resource)
                    .ThenInclude(r => r!.ResourcesImages)
                .FirstOrDefaultAsync(i => i.ItineraryItemId == id);

            if (item == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. 💡 安全檢查：確認 Resource 是否存在，且只有目前細項在使用它
                if (item.Resource != null)
                {
                    var usageCount = await _context.TripItineraryItems
                                        .CountAsync(x => x.ResourceId == item.ResourceId);

                    if (usageCount <= 1)
                    {
                        // A. 刪除實體檔案 (加上 ?. 避免 ResourcesImages 為空時出錯)
                        var images = item.Resource.ResourcesImages;
                        if (images != null)
                        {
                            foreach (var img in images)
                            {
                                // 確保路徑不為空才執行
                                if (!string.IsNullOrEmpty(img.MainImage))
                                {
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.MainImage.TrimStart('/'));
                                    if (File.Exists(filePath)) File.Delete(filePath);
                                }
                            }
                            // B. 移除圖片記錄
                            _context.ResourcesImages.RemoveRange(images);
                        }

                        // C. 移除資源記錄
                        _context.Resources.Remove(item.Resource);
                    }
                }

                // 3. 最後移除行程細項本身
                _context.TripItineraryItems.Remove(item);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        //修改細項內容的方法
        public async Task<bool> IUpdate(ViewModelTripItineraryItems items)
        {
            // 1. 取得主表資料
            var itinerary = await _context.TripItineraryItems.FindAsync(items.ItineraryItemId);
            if (itinerary == null) return false;

            // 2. 更新資源主表 (名稱與描述)
            var resource = await _context.Resources.FindAsync(itinerary.ResourceId);
            if (resource != null)
            {
                resource.ResourceName = items.ResourceName;
                resource.DefaultDescription = items.ResourceDescription;
            }

            // 3. 更新行程細項主表欄位
            itinerary.DayNumber = items.DayNumber;
            itinerary.SortOrder = items.SortOrder;
            itinerary.CustomText = items.CustomText;
            itinerary.AttractionId = items.AttractionId;
            itinerary.ActivityId = items.ActivityId;

            // 💡 4. 關鍵修正：增加「Any(f => f.Length > 0)」判斷
            // 這樣沒選新檔案時，就不會執行內部的 RemoveRange，舊圖就不會消失
            if (items.FileImages != null && items.FileImages.Any(f => f.Length > 0))
            {
                // A. 找出該資源舊有的所有圖片
                var oldImages = _context.ResourcesImages.Where(img => img.ResourceId == itinerary.ResourceId).ToList();

                // B. 刪除實體檔案 (避免垃圾文件堆積)
                foreach (var oldImg in oldImages)
                {
                    // 💡 修正關鍵：檢查 MainImage 是否有值，並使用 ! 強制告知編譯器不為 null
                    if (!string.IsNullOrEmpty(oldImg.MainImage))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PImages", oldImg.MainImage);

                        if (File.Exists(oldPath))
                        {
                            File.Delete(oldPath);
                        }
                    }
                }

                // C. 移除舊紀錄並存入新圖
                _context.ResourcesImages.RemoveRange(oldImages);

                foreach (var file in items.FileImages)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PImages", fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _context.ResourcesImages.Add(new ResourcesImage
                        {
                            ResourceId = itinerary.ResourceId,
                            MainImage = fileName
                        });
                    }
                }
            }

            // 5. 存檔
            return await _context.SaveChangesAsync() > 0;
        }
        //準備新增細項頁面所需的下拉選單與基礎資料
        public async Task<ViewModelTripItineraryItems> PrepareViewModel(int tripProductId)
        {
            var vm = new ViewModelTripItineraryItems();
            vm.TripProductId = tripProductId;

            // 💡 1. 優先抓取下拉選單 (放在最前面，最安全)
            // 這樣即使後面 Clear 快取，選單已經在 vm 裡面了
            vm.AttractionList = await _context.Attractions
                .AsNoTracking()
                .Select(s => new SelectListItem { Value = s.AttractionId.ToString(), Text = s.Name })
                .ToListAsync();

            vm.ActivityList = await _context.Activities
                .AsNoTracking()
                .Select(s => new SelectListItem { Value = s.ActivityId.ToString(), Text = s.Title })
                .ToListAsync();

            // 💡 2. 處理快取同步
            // 此時再清空快取，確保接下來抓的 TripProducts 和 ItineraryItems 是最新的
            _context.ChangeTracker.Clear();
            await Task.Delay(50); // 稍微縮短延遲，加快反應速度

            // 💡 3. 抓取產品資訊
            var product = await _context.TripProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TripProductId == tripProductId);

            vm.MaxDays = product?.DurationDays ?? 1;
            if (vm.MaxDays <= 0) vm.MaxDays = 1;

            // 💡 4. 抓取已存在的細項
            var allItems = await _context.TripItineraryItems
                .AsNoTracking()
                .Where(x => x.TripProductId == tripProductId)
                .ToListAsync();

            // 💡 5. 計算自動序位邏輯
            bool found = false;
            for (int d = 1; d <= vm.MaxDays; d++)
            {
                var usedSorts = allItems
                    .Where(x => x.DayNumber == d)
                    .Select(x => x.SortOrder)
                    .ToList();

                for (int s = 1; s <= 3; s++)
                {
                    if (!usedSorts.Contains(s))
                    {
                        vm.DayNumber = d;
                        vm.SortOrder = s;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }

            if (!found) vm.DayNumber = 0;

            return vm;
        }
        //行程細項抓圖的方法
        public async Task<ResourceDetailDto> GetResourceDetailAsync(string type, int id)
        {
            var result = new ResourceDetailDto();

            try
            {
                // 1. 處理景點類型
                if (string.Equals(type, "attraction", StringComparison.OrdinalIgnoreCase))
                {
                    result.Description = ""; // 景點沒描述

                    // 💡 根據 image_e09bcb 修正：直接使用 DbSet<Image> 並 AsNoTracking
                    result.Images = await _context.Images
                        .AsNoTracking()
                        .Where(img => img.AttractionId == id && img.ImagePath != null)
                        .Select(img => img.ImagePath!)
                        .ToListAsync();
                }
                // 2. 處理活動類型
                else if (string.Equals(type, "activity", StringComparison.OrdinalIgnoreCase))
                {
                    // 💡 抓取活動描述 (之前 Debug 證明此段成功)
                    result.Description = await _context.Activities
                        .Where(a => a.ActivityId == id)
                        .Select(a => a.Description)
                        .FirstOrDefaultAsync() ?? "";

                    // 💡 抓取活動圖片：使用獨立 Try-Catch 防止圖片查詢崩潰影響描述顯示
                    try
                    {
                        // 直接使用 DbContext 裡的 ActivityImages DbSet
                        result.Images = await _context.ActivityImages
                         .AsNoTracking()
                         .Where(i => i.ActivityId == id)
                         // 💡 請確認這裡是 ImageUrl 還是 ImagePath？
                         .Select(i => i.ImageUrl ?? "")
                         .ToListAsync();
                    }
                    catch
                    {
                        result.Images = new List<string>(); // 圖片掛掉時，至少要保住描述
                    }
                }
            }
            catch (Exception)
            {
                // 頂層防護：確保不論如何，前端都能收到一個空的 DTO 物件而不報 500
                if (string.IsNullOrEmpty(result.Description)) result.Description = "暫無描述內容";
                result.Images ??= new List<string>();
            }

            return result;
        }
        //這裡是抓行程細項內容的方法
        public async Task<ViewModelTripItineraryItems?> GetEditViewModel(int id)
        {
            // 1. 先抓出那筆要編輯的細項資料
            var item = await _context.TripItineraryItems.FindAsync(id);
            if (item == null) return null;

            // 2. 抓出關聯的資源資料 (包含名稱與描述)
            var resource = await _context.Resources.FindAsync(item.ResourceId);

            // 3. 利用你原本寫好的 PrepareViewModel 來初始化選單 (AttractionList 等)
            var vm = await PrepareViewModel(item.TripProductId);

            // 4. 把資料庫的值填入 ViewModel
            vm.ItineraryItemId = item.ItineraryItemId; // 💡 這行最重要，決定了它是編輯還是新增
            vm.TripProductId = item.TripProductId;
            vm.ResourceId = item.ResourceId;
            vm.DayNumber = item.DayNumber;
            vm.SortOrder = item.SortOrder;
            vm.CustomText = item.CustomText;
            vm.AttractionId = item.AttractionId;
            vm.ActivityId = item.ActivityId;

            if (resource != null)
            {
                vm.ResourceName = resource.ResourceName ?? "未命名資源";
                vm.ResourceDescription = resource.DefaultDescription ?? "";
                vm.ResourceId = resource.ResourceId;
            }

            // 5. 順便抓出舊有的圖片路徑，讓頁面可以預覽
            vm.AllImagePaths = _context.ResourcesImages
                     .Where(img => img.ResourceId == item.ResourceId && img.MainImage != null)
                     .Select(img => img.MainImage!)
                     .ToList() ?? new List<string>();

            return vm;
        }
        //抓取商品ID的方法
        public async Task<TripItineraryItem?> GetById(int id)
        {
          return await _context.TripItineraryItems.FindAsync(id);
        }
    }
    // 💡 寫在同一個 namespace 下，不用另外開檔
    public class ResourceDetailDto
    {
        public string Description { get; set; } = "";
        public List<string> Images { get; set; } = new List<string>();
    }

}
