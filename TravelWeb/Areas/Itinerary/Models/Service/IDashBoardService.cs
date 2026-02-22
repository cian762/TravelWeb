using TravelWeb.Areas.Itinerary.Models.ViewModel;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public interface IDashBoardService
    {
      Task<  DashBoardViewModel> GetDashboardData();


    }
}
