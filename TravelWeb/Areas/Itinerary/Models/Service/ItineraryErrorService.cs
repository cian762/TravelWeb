using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.ViewModel;
using TravelWeb.Areas.Itinerary.Repository;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public class ItineraryErrorService : IItineraryErrorSevice
    {
        private readonly IItineraryGenericRepository<AigenerationError> _errorRepo;

        public ItineraryErrorService(IItineraryGenericRepository<AigenerationError> errorRepo)
        {
            _errorRepo = errorRepo;
        }

        public async Task<ErrorViewModel> GetAllErrorsAsync()
        {
            var errors = _errorRepo.GetAll()
                .OrderByDescending(x => x.CreateTime)
                .ToList();

            return new ErrorViewModel
            {
                Errors = errors.Select(e => new ItineraryErrorItemVM
                {
                    ErrorId = e.ErrorId,
                    ItineraryId = e.ItineraryId,
                    VersionId = e.VersionId.Value,
                    ErrorType = e.ErrorType,
                    ErrorMessage = e.ErrorMessage,
                    CreatedTime = e.CreateTime.Value,
                    IsConfirmed = e.IsConfirmed,
                    ErrorReason = e.ErrorReason,
                    AdminResponse = e.AdminResponse
                }).ToList()
            };
        }

        public async Task<EditItineraryErrorViewModel> GetErrorByIdAsync(int id)
        {
            var e = _errorRepo.GetAll()
                .FirstOrDefault(x => x.ErrorId == id);

            return new EditItineraryErrorViewModel
            {
                ErrorId = e.ErrorId,
                ErrorType = e.ErrorType,
                ErrorMessage = e.ErrorMessage,
                CreateTime = e.CreateTime.Value,
                IsConfirmed = e.IsConfirmed,
                ErrorReason = e.ErrorReason,
                AdminResponse = e.AdminResponse
            };
        }

        public async Task UpdateErrorAsync(EditItineraryErrorViewModel model)
        {
            var entity = _errorRepo.GetAll()
                .FirstOrDefault(x => x.ErrorId == model.ErrorId);

            entity.IsConfirmed = model.IsConfirmed;
            entity.ErrorReason = model.ErrorReason;
            entity.AdminResponse = model.AdminResponse;

            _errorRepo.Update(entity);
        }
    }
}
