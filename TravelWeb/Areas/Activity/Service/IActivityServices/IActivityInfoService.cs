using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IActivityInfoService
    {
        Task<IEnumerable<ActivityInfoViewModel>> GetAllActInfoAsync();

        Task<ActivityInfoViewModel?> GetActInfoByIdAsync(int id);

        Task CreateActInfoAsync(ActivityInfoViewModel vm, List<IFormFile> images);

        Task EditActInfoAsync(ActivityInfoViewModel vm, List<IFormFile> images, List<string> imageDeleteUrls);

        Task DeleteActInfoAsync(int id);

        List<string> ProvideTypeTag();
    
        List<string> ProvideRegionTag();
    }
}
