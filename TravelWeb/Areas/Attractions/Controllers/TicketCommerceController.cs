using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Attractions.Controllers
{
    public class TicketCommerceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
