using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Attractions.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult TripIndex()
        {
            return View();
        }
    }
}
