using CloudinaryDotNet.Actions;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Repository.IActivityRepositories
{
    public interface IActivityInfoRepository
    {
        IQueryable<Models.EFModel.Activity> Get();

        Task CreateAsync(ActivityInfoViewModel vm, List<ImageUploadResult> imageUpoladDetails);

        Task UpdateAsync(ActivityInfoViewModel vm, List<ImageUploadResult> imageUploadDetails, List<string> imageDeleteUrls);

        Task DeleteAsync();

        Task SaveChangeAsync();
        List<string>? FindPublicId(List<string> imageDeleteUrls);

        List<string> GetTypeTag();

        List<string> GetRegionTag();
    }
}
