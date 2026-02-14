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

        

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Models.TripProduct>> GetAll()
        {
          return await _context.TripProducts.ToArrayAsync();
        }

        public async Task<IEnumerable<TripIndexViewModel>> GetAllForIndex()
        {
          var q=_context.TripProducts
         .Include(p => p.Region)
         .Select(p => new TripIndexViewModel
         {
             TripProductId = p.TripProductId,
             ProductName = p.ProductName,
             ImagePath = string.IsNullOrEmpty(p.CoverImage) ? "/images/no-pic.jpg" : "/PImages/" + p.CoverImage,
             RegionName = p.Region != null ? p.Region.RegionName : "未分類",
             DisplayPrice = p.DisplayPrice.HasValue ? p.DisplayPrice.Value.ToString("C0") : "未定價",
             Status = p.Status
         }).ToListAsync();
            return await q;
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

        public Task<bool> Update(ViewModelProducts vm)
        {
            throw new NotImplementedException();
        }
    }
}
