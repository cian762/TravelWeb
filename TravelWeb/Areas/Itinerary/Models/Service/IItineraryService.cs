using TravelWeb.Areas.Itinerary.Models.ViewModel;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public interface IItineraryService
    {
        Task<ItineraryDetailViewModel> GetItineraryManagementAsync();
        Task<VersionViewModel> GetVersionManagementAsync(int itineraryId);
    }
}
