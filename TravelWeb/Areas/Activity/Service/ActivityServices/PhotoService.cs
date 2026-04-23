using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TravelWeb.Areas.Activity.Models;
using TravelWeb.Areas.Activity.Service.IActivityServices;

namespace TravelWeb.Areas.Activity.Service.ActivityServices
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName,config.Value.ApiKey,config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<List<ImageUploadResult>> AddPhotoAsync(List<IFormFile> file)
        {
            var uploadResult = new ImageUploadResult();
            var resultCollection = new List<ImageUploadResult>();

            if (file != null && file.Any()) 
            {
                foreach (var f in file) 
                {
                    using var stream = f.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(f.FileName, stream),
                        Transformation = new Transformation().Width(1000).Crop("fit")
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    resultCollection.Add(uploadResult);
                }
            }
            return resultCollection;
        }

        public async Task DeletePhotoAsync(List<string>? publicIds)
        {

            if (publicIds == null || !publicIds.Any()) return;

            var delParams = new DelResParams()
            {
                PublicIds = publicIds,
                Type = "upload",
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DeleteResourcesAsync(delParams);

        }
    }
}
