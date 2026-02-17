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
        private readonly ITripItineraryItem _item;
        public TripController(ITripproducts context, ITripItineraryItem item)
        { 
         _context = context;
            _item = item;
        }
        //這裡是行程商品
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
            else
            {
              products = await _context.GetCreateViewModelAsync();
              return View(products);
            }
          
        }
        //[HttpGet]
        //public async Task<IActionResult> UpProduct(ViewModelProducts vm)
        //{
        //    return View(vm);
            
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpProduct(ViewModelProducts vm)
        //{
        //    return View(vm); 
        //}
        //這裡是行程細項
        public async Task <IActionResult> ItemIndex(int id)
        {
            var q = await _item.IGetAny(id);
            if (q == null || !q.Any())
            {
                // 沒資料，直接跳轉到新增頁面，並帶上商品 ID
                return RedirectToAction("CreateItem", new { id = id });
            }

            // 有資料，才顯示檢視頁面
            return View(q);
        }
        [HttpGet]
        public async Task<IActionResult> CreateItem(int id)
        { 
         var vm=await _item.PrepareViewModel(id);
            return View(vm);
        }
       
    }
}
