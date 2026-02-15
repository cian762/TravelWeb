using TravelWeb.Areas.Itinerary.Models.ViewModel;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public interface IItineraryErrorSevice
    {
        Task<ErrorViewModel> GetAllErrorsAsync();
        Task<EditItineraryErrorViewModel> GetErrorByIdAsync(int id);
        Task UpdateErrorAsync(EditItineraryErrorViewModel model);
    }
}
