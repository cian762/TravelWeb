using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class TripController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
