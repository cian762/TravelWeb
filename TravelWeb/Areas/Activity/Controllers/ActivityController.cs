using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.Activity.Controllers
{
    [Area("Activity")]
    [Route("Act")]
    public class ActivityController : Controller
    {
        [HttpGet("Home/{num?}")]
        public IActionResult Index(int num)
        {
            return Content($"路由來的: {num}");
        }
    }
}
