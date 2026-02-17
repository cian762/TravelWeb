namespace TravelWeb.Areas.Itinerary.Models.ViewModel
{
    public class DashBoardViewModel
    {
        public int TotalItineraries { get; set; }
        public int TodayItineraries { get; set; }
        public int TotalVersions { get; set; }
        public int TodayAIGenerations { get; set; }
        public double AvgFeasibility { get; set; }
        public int UnconfirmedErrors { get; set; }

        public List<ActivityViewModel> RecentActivities { get; set; }
    }
}
