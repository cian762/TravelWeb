using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Attractions.Controllers
{
    public class RecycleBinController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
