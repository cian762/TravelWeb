using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelWeb.Areas.Itinerary.Models.Service;

namespace TravelWeb.Areas.Itinerary
{
    [Area("Itinerary")]
    public class ItineraryController : Controller
    {
        private readonly IDashBoardService _dashboardService;
        private readonly IItineraryService _itineraryService;
        private readonly IItineraryErrorSevice _errorSevice;
        public ItineraryController(IDashBoardService dashBoardService,IItineraryService itineraryService,IItineraryErrorSevice errorSevice)
        {
            _dashboardService = dashBoardService;
            _itineraryService = itineraryService;
            _errorSevice = errorSevice;
        }
        public async Task<IActionResult> ItineraryError()
        {
            var model =await  _errorSevice.GetAllErrorsAsync();
            return View(model);
        }
        public async Task< IActionResult >ItineraryErrorEdit(int id)
        {
        var model = await _errorSevice.GetErrorByIdAsync(id);
            return View(model);
        }
        public IActionResult ItineraryCompareAnalyze()
        {
            return View();
        }
        public IActionResult ItineraryDashBoard()
        {
            var model = _dashboardService.GetDashboardData();
            return View(model);
        }
        public async Task< IActionResult> ItineraryManage()
        {
            var model = await  _itineraryService.GetItineraryManagementAsync();
            return View(model);
        }
        public async Task< IActionResult> ItineraryVersionManage(int id)
        {
            var model = await _itineraryService.GetVersionManagementAsync(id);
            return View();
        }
        public IActionResult AIAnalyze()
        {
            return View();
        }


    }
}
