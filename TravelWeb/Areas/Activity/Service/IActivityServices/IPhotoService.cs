using CloudinaryDotNet.Actions;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IPhotoService
    {
        Task<List<ImageUploadResult>> AddPhotoAsync(List<IFormFile> file);

        Task DeletePhotoAsync(List<string>? publicIds);
    }
}
