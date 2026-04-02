using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IActivityInfoService
    {
        Task<IEnumerable<ActivityInfoViewModel>> GetAllActInfo();

        Task<ActivityInfoViewModel?> GetActInfoById(int id);

        Task CreateActInfo(ActivityInfoViewModel vm, List<IFormFile> images);

        Task EditActInfo(ActivityInfoViewModel vm, List<IFormFile> images, List<string> imageDeleteUrls);

        Task DeleteActInfo();

        List<string> ProvideTypeTag();
    
        List<string> ProvideRegionTag();
    }
}
