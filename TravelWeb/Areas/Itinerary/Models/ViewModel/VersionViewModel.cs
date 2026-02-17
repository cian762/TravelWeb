    namespace TravelWeb.Areas.Itinerary.Models.ViewModel
{
    public class VersionViewModel
    {
        public string ItineraryTitle { get; set; }
        public int ItineraryId { get; set; }

        public List<VersionItemVM> Versions { get; set; }
    }
    public class VersionItemVM
    {
        public int VersionId { get; set; }
        public int VersionNumber { get; set; }
        public string Creator { get; set; }
        public string CurrentUsageStatus { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
