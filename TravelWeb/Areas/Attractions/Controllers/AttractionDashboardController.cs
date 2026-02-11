using Microsoft.AspNetCore.Mvc;
// 測試上傳喔喔喔喔
namespace TravelWeb.Areas.Attractions.Controllers
{
    [Area("Attractions")]
    public class AttractionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    

}
}
