using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.TripProduct.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
