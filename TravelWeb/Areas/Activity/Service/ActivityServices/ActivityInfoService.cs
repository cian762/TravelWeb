using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Repository.IActivityRepositories;
using TravelWeb.Areas.Activity.Service.IActivityServices;

namespace TravelWeb.Areas.Activity.Service.ActivityServices
{
    public class ActivityInfoService : IActivityInfoService
    {
        private readonly IActivityInfoRepository _activityInfoRepository;
        private readonly IPhotoService _photoService;

        public ActivityInfoService(IActivityInfoRepository activityInfoRepository, IPhotoService photoService)
        {
            _activityInfoRepository = activityInfoRepository;
            _photoService = photoService;
        }

        public async Task<IEnumerable<ActivityInfoViewModel>> GetAllActInfo()
        {
            var vm = await _activityInfoRepository.Get()
                                .Select(m => new ActivityInfoViewModel
                                {
                                    ActivityId = m.ActivityId,
                                    Title = m.Title,
                                    StartTime = m.StartTime,
                                    EndTime = m.EndTime,
                                    Address = m.Address,
                                    OfficialLink = m.OfficialLink,
                                    UpdateAt = m.UpdateAt,
                                    RegionName = m.Regions.Select(r => r.RegionName).ToList(),
                                    TypeName = m.Types.Select(t => t.ActivityType).ToList()
                                })
                                .ToListAsync();
            return vm;
        }

        public async Task<ActivityInfoViewModel?> GetActInfoById(int id)
        {

            if (_activityInfoRepository.Get().Any(a => a.ActivityId == id) != false) 
            {
                var act = await _activityInfoRepository.Get()
                .Where(a => a.ActivityId == id)
                .Select(m => new ActivityInfoViewModel
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
                    ImgUrls = m.ActivityImages.Where(i => i.ActivityId == m.ActivityId).Select(u => u.ImageUrl).ToList()!,
                }).FirstOrDefaultAsync();

                return act;

            }

            return null;
        }

        public async Task CreateActInfo(ActivityInfoViewModel vm, List<IFormFile> images)
        {
            var imageDetails = await _photoService.AddPhotoAsync(images);
            await _activityInfoRepository.CreateAsync(vm, imageDetails);
            await _activityInfoRepository.SaveChangeAsync();

        }

        public async Task EditActInfo(ActivityInfoViewModel vm, List<IFormFile> images, List<string> imageDeleteUrls)
        {
            var imageDetails = await _photoService.AddPhotoAsync(images);

            var publicIds = _activityInfoRepository.FindPublicId(imageDeleteUrls);
            await _photoService.DeletePhotoAsync(publicIds);
            await _activityInfoRepository.UpdateAsync(vm, imageDetails, imageDeleteUrls);
            await _activityInfoRepository.SaveChangeAsync();
        }

        public Task DeleteActInfo()
        {
            throw new NotImplementedException();
        }


        public List<string> ProvideTypeTag() 
        {
           return _activityInfoRepository.GetTypeTag();
        }

        public List<string> ProvideRegionTag()
        {
            return _activityInfoRepository.GetRegionTag();
        }


    }
}
