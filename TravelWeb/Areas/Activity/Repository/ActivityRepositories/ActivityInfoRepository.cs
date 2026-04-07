using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Repository.IActivityRepositories;
using TravelWeb.Areas.Activity.Service.ActivityServices;
using TravelWeb.Areas.Activity.Service.IActivityServices;
namespace TravelWeb.Areas.Activity.Repository.ActivityRepositories
{
    public class ActivityInfoRepository : IActivityInfoRepository
    {
        private readonly ActivityDbContext _dbContext;


        public ActivityInfoRepository(ActivityDbContext dbContext)
        {
            _dbContext = dbContext;
            
        }

        public IQueryable<Models.EFModel.Activity> Get()
        {
            var acts = _dbContext.Activities
                .Where(a => a.SoftDelete == false)
                .AsSplitQuery();
            return acts;
        }


        public async Task CreateAsync(ActivityInfoViewModel vm, List<ImageUploadResult> imageDetails)
        {
            //投影 vm 到 EF Model
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

            //建立活動與活動類型的關聯
            if (vm.TypeName != null && vm.TypeName.Any())
            {
                var selectedTypes = await _dbContext.TagsActivityTypes
                                        .Where(t => vm.TypeName.Contains(t.ActivityType))
                                        .ToListAsync();
                foreach (var t in selectedTypes)
                {
                    act.Types.Add(t);
                }
            }

            //建立活動與地區的關聯
            if (vm.RegionName != null && vm.RegionName.Any())
            {
                var selectedRegions = await _dbContext.TagsRegions
                                          .Where(r => vm.RegionName.Contains(r.RegionName))
                                          .ToListAsync();
                foreach (var r in selectedRegions)
                {
                    act.Regions.Add(r);
                }
            }

            //照片存取到雲端，資料庫只存網址
            if (imageDetails != null && imageDetails.Count > 0)
            {
                foreach (var detail in imageDetails)
                {
                    act.ActivityImages.Add(new ActivityImage
                    {
                        ImageUrl = detail.SecureUrl.AbsoluteUri,
                        PublicId = detail.PublicId,
                    });
                }
            }

            act.SoftDelete = false;
            _dbContext.Activities.Add(act);
        }

        public async Task UpdateAsync(ActivityInfoViewModel vm, List<ImageUploadResult> imageUploadDetails, List<string> imageDeleteUrls)
        {
            var act = await _dbContext.Activities
                    .Include(a => a.Types)
                    .Include(a => a.Regions)
                    .Include(a => a.ActivityImages)
                    .FirstOrDefaultAsync(a => a.ActivityId == vm.ActivityId);

            if (act != null) 
            {
                act.Title = vm.Title;
                act.StartTime = vm.StartTime;
                act.EndTime = vm.EndTime;
                act.OfficialLink = vm.OfficialLink;
                act.Address = vm.Address;
                act.Description = vm.Description;

                // 更新活動類型的關聯
                act.Types.Clear();

                if (vm.TypeName != null && vm.TypeName.Any())
                {
                    var selectedTypes = await _dbContext.TagsActivityTypes
                    .Where(t => vm.TypeName.Contains(t.ActivityType))
                    .ToListAsync();

                    foreach (var t in selectedTypes)
                    {
                        act.Types.Add(t); 
                    }
                }

                // 更新地區的關聯
                act.Regions.Clear();

                if (vm.RegionName != null && vm.RegionName.Any())
                {
                    var selectedRegions = await _dbContext.TagsRegions
                    .Where(r => vm.RegionName.Contains(r.RegionName))
                    .ToListAsync();

                    foreach (var r in selectedRegions)
                    {
                        act.Regions.Add(r); 
                    }
                }

                // 新增圖片資訊
                if (imageUploadDetails != null && imageUploadDetails.Count > 0)
                {
                    foreach (var detail in imageUploadDetails)
                    {
                        act.ActivityImages.Add(new ActivityImage
                        {
                            ImageUrl = detail.SecureUrl.AbsoluteUri,
                            PublicId = detail.PublicId,
                        });
                    }
                }


                // 移除圖片資訊
                if (imageDeleteUrls != null && imageDeleteUrls.Any())
                {
                    foreach (var url in imageDeleteUrls)
                    {
                        // 從已經 Include 的集合中找，減少資料庫查詢次數
                        var photoToRemove = act.ActivityImages.FirstOrDefault(u => u.ImageUrl == url);

                        if (photoToRemove != null)
                        {
                            _dbContext.ActivityImages.Remove(photoToRemove);
                        }
                    }
                }

                act.UpdateAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var act = await _dbContext.Activities.FirstOrDefaultAsync(a => a.ActivityId == id);

            if (act != null) 
            {
                act.SoftDelete = true;
            }
        }


        public async Task SaveChangeAsync()
        {
           await _dbContext.SaveChangesAsync();
        }


        public List<string> GetTypeTag() 
        {
            return _dbContext.TagsActivityTypes
                .Select(m => m.ActivityType)
                .ToList();
            
        }

        public List<string> GetRegionTag()
        {
            return _dbContext.TagsRegions
                .Where(m => m.Uid == null)
                .Select(m => m.RegionName)
                .ToList();
        }

        public List<string>? FindPublicId(List<string> imageDeleteUrls)
        {
            if (imageDeleteUrls == null) 
            {
                return new List<string>();
            }
            return _dbContext.ActivityImages
                 .Where(i => imageDeleteUrls.Contains(i.ImageUrl!))
                 .Select(i => i.PublicId)
                 .ToList()!;
        }
    }
}
