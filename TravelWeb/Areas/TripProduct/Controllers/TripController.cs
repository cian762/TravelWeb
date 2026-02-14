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
        public async Task<IActionResult> Index()
        {
            
            var products = await _context.GetAllForIndex();
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> CreatProduct()
        {
            var viewmodel = await _context.GetCreateViewModelAsync();
            return View(viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatProduct(ViewModelProducts products)
        {
            if (ModelState.IsValid && await _context.Create(products))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
    }
}
