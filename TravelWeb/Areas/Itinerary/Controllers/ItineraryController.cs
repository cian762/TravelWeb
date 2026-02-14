using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.Itinerary.Models.Service;

namespace TravelWeb.Areas.Itinerary
{
    [Area("Itinerary")]
    public class ItineraryController : Controller
    {
        private readonly IDashBoardService _dashboardService;
        public ItineraryController(IDashBoardService dashBoardService)
        {
            _dashboardService = dashBoardService;
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
            return View();
        }
        public IActionResult ItineraryVersionManage()
        {
            return View();
        }
        public IActionResult AIAnalyze()
        {
            return View();
        }


    }
}
