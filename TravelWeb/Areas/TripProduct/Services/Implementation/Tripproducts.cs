using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;

namespace TravelWeb.Areas.TripProduct.Services.Implementation
{
    public class Tripproducts : ITripproducts
    {
        private readonly TripDbContext _context;
       public Tripproducts(TripDbContext context)
        {
           _context = context;
        }
        //這是行程商品新增的方法
        public async Task<bool> Create(ViewModelProducts vm)
        {
            var trip = new TravelWeb.Areas.TripProduct.Models.TripProduct();
            trip.ProductName = vm.ProductName;
            trip.DisplayPrice = vm.DisplayPrice;
            trip.Description = vm.Description;
            trip.DurationDays = vm.DurationDays;
            trip.ClickTimes = 0;
            trip.Status = "上架";
            trip.RegionId = vm.RegionId;
            trip.PolicyId = vm.PolicyId;
            if (vm.ImageFile != null)
            {
                string folder = Path.Combine("wwwroot", "PImages");
                string filename=Guid.NewGuid().ToString()+" "+vm.ImageFile.FileName;
                string filepath= Path.Combine(folder, filename);
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                  await vm.ImageFile.CopyToAsync(stream);
                }
                trip.CoverImage = filename;
            }
            if (vm.SelectedTags != null && vm.SelectedTags.Any())
            {
                trip.TravelTags = new List<TravelTag>();
                foreach (var tagId in vm.SelectedTags)
                {
                    // 如果 tagId 是數字字串，代表是選的
                    if (int.TryParse(tagId, out int id))
                    {
                        var tag = await _context.TravelTags.FindAsync(id);
                        if (tag != null) trip.TravelTags.Add(tag);
                    }
                   
                    else 
                    {
                      var newTag = new TravelTag { TravelTagName = tagId };
                      trip.TravelTags.Add(newTag);
                    }
                }
            }
            _context.TripProducts.Add(trip);
            // 這裡存檔，會同時存入商品和中間表的關聯
            return await _context.SaveChangesAsync() > 0;
        }

        
        //這是刪除行程商品的方法
        public async Task<string> Delete(int id)
        {
            // 💡 只需要找主體就好，完全不需要 Include 其他表，這樣效能最快
            var trip = await _context.TripProducts.FindAsync(id);

            if (trip == null) return "NotFound";

            // 💡 統一改狀態為「已刪除紀錄」
            // 這樣它就會被後台 Index 的 .Where(p => p.Status != "已刪除紀錄") 濾掉
            trip.Status = "已刪除紀錄";

            _context.Update(trip);
            await _context.SaveChangesAsync();

            return "SoftDeleted";
        }

        //這是抓所有行程商品的方法
        public async Task<IEnumerable<Models.TripProduct>> GetAll()
        {
          return await _context.TripProducts.ToArrayAsync();
        }

        //這是顯示行程主表首頁的方法
        public async Task<(IEnumerable<TripIndexViewModel> List, int TotalCount)> GetAllForIndex(string? keyword = null, int? regionId = null,string ?status = null,  int page = 1)
        {
            int pageSize = 10;

            // 1. 永遠先過濾掉「軟刪除」的資料
            var query = _context.TripProducts
                .Include(p => p.Region)
                .Where(p => p.Status != "已刪除紀錄");

            // 2. 遞增條件：有打字才搜
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.ProductName!.Contains(keyword));
            }

            // 3. 遞增條件：有選地區才搜
            if (regionId.HasValue)
            {
                query = query.Where(p => p.RegionId == regionId.Value);
            }

            // 4. 遞增條件：有選狀態才搜
            if (!string.IsNullOrEmpty(status) && status != "--全部--")
            {
                query = query.Where(p => p.Status == status);
            }

            // 5. 算出符合這堆條件的總筆數
            int totalCount = await query.CountAsync();

            // 6. 分頁抓資料
            var list = await query
                .OrderBy(p => p.TripProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new TripIndexViewModel
                {
                    TripProductId = p.TripProductId,
                    ProductName = p.ProductName,
                    ImagePath = string.IsNullOrEmpty(p.CoverImage) ? "/images/no-pic.jpg" : "/PImages/" + p.CoverImage,
                    RegionName = p.Region != null ? p.Region.RegionName : "未分類",
                    DisplayPrice = p.DisplayPrice.HasValue ? p.DisplayPrice.Value.ToString("C0") : "未定價",
                    Status = p.Status
                }).ToListAsync();

            return (list, totalCount);
        }

        //這是在商品主表填入地區 規則 標籤的方法
        public async Task<ViewModelProducts> GetCreateViewModelAsync()
        {
            var vm = new ViewModelProducts();
            vm.RegionOptions = await _context.TripRegions
                .Select(r => new SelectListItem
                {
                    Value = r.RegionId.ToString(), 
                    Text = r.RegionName            
                }).ToListAsync();
            vm.PolicyOptions = await _context.CancellationPolicies
        .Select(p => new SelectListItem
        {
            Value = p.PolicyId.ToString(),
            Text = $"{p.PolicyName}({p.Description})" 
        }).ToListAsync();

            vm.TagOptions = await _context.TravelTags
                .Select(t => new SelectListItem
                {
                    Value = t.TravelTagid.ToString(), 
                    Text = t.TravelTagName
                })
                .ToListAsync();
            return vm;

        }
        //這是行程商品修改的方法
        public async Task<bool> Update(ViewModelProducts vm)
        {
            var q = await _context.TripProducts
         .Include(p => p.Region)
         .Include(p => p.Policy) //
         .Include(p => p.TravelTags)
         .FirstOrDefaultAsync(t => t.TripProductId == vm.TripProductId);

            if (q == null) return false; //

            // 2. 💡 先備份舊天數，用於後續比對刪除邏輯
            int oldDays = q.DurationDays ?? 1;

            // 3. 處理圖片更新與舊圖刪除
            if (vm.ImageFile != null)
            {
                // 💡 如果原本有舊圖，先從硬碟刪除，避免浪費空間
                if (!string.IsNullOrEmpty(q.CoverImage))
                {
                    string oldPath = Path.Combine("wwwroot", "PImages", q.CoverImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                string folder = Path.Combine("wwwroot", "PImages");
                string filename = Guid.NewGuid().ToString() + "_" + vm.ImageFile.FileName;
                string filepath = Path.Combine(folder, filename);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(stream);
                }
                q.CoverImage = filename;
            }

            // 4. 更新基本欄位 (放在天數刪除邏輯之後或記錄 oldDays 後)
            q.ProductName = vm.ProductName;
            q.DurationDays = vm.DurationDays;
            q.DisplayPrice = vm.DisplayPrice;
            q.Description = vm.Description;
            q.RegionId = vm.RegionId;
            q.PolicyId = vm.PolicyId;
            q.Status = vm.Status;

            // 5. 更新標籤 (先清空再重新加入)
            q.TravelTags.Clear();
            if (vm.SelectedTags != null && vm.SelectedTags.Any())
            {
                foreach (var tagId in vm.SelectedTags)
                {
                    if (int.TryParse(tagId, out int id))
                    {
                        var tag = await _context.TravelTags.FindAsync(id);
                        if (tag != null) q.TravelTags.Add(tag);
                    }
                    else
                    {
                        // 處理手動輸入的新標籤
                        var newTag = new TravelTag { TravelTagName = tagId };
                        q.TravelTags.Add(newTag);
                    }
                }
            }

            // 6. 💡 處理天數縮減：如果新天數比舊天數短，刪除多出的行程細項
            // 注意：必須使用我們備份的 oldDays 來比對
            if (vm.DurationDays < oldDays)
            {
                var itemsToRemove = _context.TripItineraryItems
                    .Where(i => i.TripProductId == vm.TripProductId && i.DayNumber > vm.DurationDays);

                _context.TripItineraryItems.RemoveRange(itemsToRemove);
            }

            // 7. 儲存至資料庫
            return await _context.SaveChangesAsync() > 0;
        }
        //這是用ID抓行程商品內容的方法
        public async Task<ViewModelProducts?> GetIdUpData(int id)
        {
            // 1. 去資料庫抓出原始資料 (包含標籤)
            var trip = await _context.TripProducts
                .Include(p => p.TravelTags)
                .FirstOrDefaultAsync(p => p.TripProductId == id);

            if (trip == null) return null;

            // 2. 把資料庫的內容填進 ViewModel
            var vm = new ViewModelProducts
            {
                TripProductId = trip.TripProductId,
                ProductName = trip.ProductName,
                DisplayPrice = trip.DisplayPrice,
                Description = trip.Description,
                DurationDays = trip.DurationDays,
                RegionId = trip.RegionId,
                PolicyId = trip.PolicyId,
                Status = trip.Status,
                // 把原本選好的標籤 ID 變成字串清單給前端
                SelectedTags = trip.TravelTags.Select(t => t.TravelTagid.ToString()).ToList()
            };

            // 3. 補上下拉選單的資料 (讓頁面能選地區、規則等)
            var options = await GetCreateViewModelAsync();
            vm.RegionOptions = options.RegionOptions;
            vm.PolicyOptions = options.PolicyOptions;
            vm.TagOptions = options.TagOptions;

            return vm;
        }

      
    }
}
