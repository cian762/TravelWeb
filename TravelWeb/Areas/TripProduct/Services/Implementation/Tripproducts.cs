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
        
        public Task<bool> Create(ViewModelProducts vm)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ViewModelProducts> GetCreateViewModelAsync()
        {
            var vm = new ViewModelProducts();

            // 2. 抓取地區選單 (TripRegions)
            // 將資料庫實體轉換為 SelectListItem 格式
            vm.RegionOptions = await _context.TripRegions
                .Select(r => new SelectListItem
                {
                    Value = r.RegionId.ToString(), // 資料庫的主鍵
                    Text = r.RegionName            // 顯示在選單上的名稱
                }).ToListAsync();
            vm.PolicyOptions = await _context.CancellationPolicies
        .Select(p => new SelectListItem
        {
            Value = p.PolicyId.ToString(),
            Text = $"{p.PolicyName}({p.Description})" // 這樣使用者就能看到內容了
        }).ToListAsync();

            // 3. 抓取標籤選單 (TravelTags)
            // 這些是「既有」的標籤，供使用者直接挑選
            vm.TagOptions = await _context.TravelTags
                .Select(t => new SelectListItem
                {
                    Value = t.TravelTagid.ToString(), // 注意大小寫
                    Text = t.TravelTagName
                })
                .ToListAsync();

            // 4. (選填) 如果你有退款政策選單，也在此處抓取
            // vm.PolicyOptions = await _context.CancellationPolicies...

            // 5. 回傳這個填滿「選項」但「輸入欄位」為空的物件
            return vm;

        }

        public Task<bool> Update(ViewModelProducts vm)
        {
            throw new NotImplementedException();
        }
    }
}
