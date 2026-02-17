using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;

namespace TravelWeb.Areas.Itinerary.Models.ViewModel
{
    public class ItineraryDetailViewModel
    {
        public List<ItineraryItemVM> Itineraries { get; set; }

      
    }
    public class ItineraryItemVM
    {
        public int ItineraryId { get; set; }
        public string Title { get; set; }
        public string CurrentVersionNumber { get; set; }
        public string CurrentUsageStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public int VersionCount { get; set; }
    }
}
