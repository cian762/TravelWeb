using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class ItineraryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
