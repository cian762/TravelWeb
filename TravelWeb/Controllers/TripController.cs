using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class TripController : Controller
    {
        public IActionResult TripIndex()
        {
            return View();
        }
    }
}
