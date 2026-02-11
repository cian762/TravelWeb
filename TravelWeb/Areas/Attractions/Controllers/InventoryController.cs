using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Attractions.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
