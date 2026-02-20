using System;
using TravelWeb.Areas.Itinerary.Models.ViewModel;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public interface IItineraryService
    {
        Task<ItineraryDetailViewModel> GetItineraryManagementAsync();
        Task<VersionViewModel> GetVersionManagementAsync(int itineraryId);
        void SetCurrentVersion(int versionId);
        DiffViewModel GetDiff(int versionId);
    }
}
