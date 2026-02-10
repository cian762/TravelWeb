using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class AttractionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
