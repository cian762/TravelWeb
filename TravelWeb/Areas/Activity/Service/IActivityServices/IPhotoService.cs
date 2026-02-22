using CloudinaryDotNet.Actions;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
