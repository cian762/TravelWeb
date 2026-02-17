using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelWeb.Areas.Itinerary.Models.Service;
using TravelWeb.Areas.Itinerary.Models.ViewModel;

namespace TravelWeb.Areas.Itinerary
{
    [Area("Itinerary")]
    public class ItineraryController : Controller
    {
        private readonly IDashBoardService _dashboardService;
        private readonly IItineraryService _itineraryService;
        private readonly IItineraryErrorSevice _errorSevice;
        private readonly IItineraryCompareService _compareservice;
        public ItineraryController(IDashBoardService dashBoardService,IItineraryService itineraryService, IItineraryErrorSevice errorSevice, IItineraryCompareService compareservice)
        {
            _dashboardService = dashBoardService;
            _itineraryService = itineraryService;
            _errorSevice = errorSevice;
            _compareservice = compareservice;
            
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
        [HttpPost]
        public async Task<IActionResult>Edit(EditItineraryErrorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _errorSevice.UpdateErrorAsync(model);

            return RedirectToAction("Index");
        }

        public IActionResult ItineraryCompareAnalyze()
        {
            var model = _compareservice.GetComparisonAnalysis();
            return View(model);
        }

        public async Task<IActionResult> ItineraryDashBoard()
        {
            var model = await  _dashboardService.GetDashboardData();
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
       


    }
}
