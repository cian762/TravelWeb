using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.Itinerary.Models.Service;

namespace TravelWeb.Areas.Itinerary
{
    [Area("Itinerary")]
    public class ItineraryController : Controller
    {
        private readonly IDashBoardService _dashboardService;
        private readonly IItineraryService _itineraryService;
        public ItineraryController(IDashBoardService dashBoardService,IItineraryService itineraryService)
        {
            _dashboardService = dashBoardService;
            _itineraryService = itineraryService;
        }
        public IActionResult ItineraryError()
        {
            return View();
        }
        public IActionResult ItineraryCompareAnalyze()
        {
            return View();
        }
        public IActionResult ItineraryDashBoard()
        {
            var model =_dashboardService.GetDashboardData();
            return View(model);
        }
        public IActionResult ItineraryManage()
        {
            var model = _itineraryService.GetItineraryManagementAsync();
            return View(model);
        }
        public IActionResult ItineraryVersionManage(int id)
        {
            var model = _itineraryService.GetVersionManagementAsync(id);
            return View();
        }
        public IActionResult AIAnalyze()
        {
            return View();
        }


    }
}
