using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class ItineraryController : Controller
    {
        [Route("Itinerary/{action}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
