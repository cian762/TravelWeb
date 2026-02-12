using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Attractions.Controllers
{
    
    public class AttractionAssetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
