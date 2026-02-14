using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;

namespace TravelWeb.Areas.TripProduct.Controllers
{
    [Area("TripProduct")]
    public class TripController : Controller
    {
        private readonly ITripproducts _context;
        public TripController(ITripproducts context)
        { 
         _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreatProduct()
        {
            var viewmodel = await _context.GetCreateViewModelAsync();
            return View(viewmodel);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreatProduct()
        //{
        //    return View();
        //}
    }
}
