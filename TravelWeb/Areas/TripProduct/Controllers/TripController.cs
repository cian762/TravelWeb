using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(string keyword, int? regionId, string status, int page = 1)
        {
            // 1. 呼叫 Service，確保傳入的是 keyword
            var (list, totalCount) = await _context.GetAllForIndex(keyword, regionId, status, page);

            // 2. 準備地區下拉選單 (使用你現有的 GetCreateViewModelAsync)
            var createVm = await _context.GetCreateViewModelAsync();
            ViewBag.Regions = new SelectList(createVm.RegionOptions, "Value", "Text", regionId?.ToString());

            // 3. 💡 關鍵：將搜尋條件存入 ViewData，讓網頁搜尋框能留住文字
            ViewData["Keyword"] = keyword;
            ViewData["RegionId"] = regionId;
            ViewData["Status"] = status;

            // 4. 分頁計算
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount /10);

            return View(list);
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
        [HttpGet]
        public async Task<IActionResult> UpProduct(int id)
        {
            var vm = await _context.GetIdUpData(id);

            if (vm == null) return NotFound();

            return View(vm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpProduct(ViewModelProducts vm)
        {
            if (!ModelState.IsValid)
            {
                // 如果驗證失敗，重新抓取下拉選單選項
                var options = await _context.GetCreateViewModelAsync();
                vm.RegionOptions = options.RegionOptions;
                vm.PolicyOptions = options.PolicyOptions;
                vm.TagOptions = options.TagOptions;
                return View(vm);
            }

            // 💡 呼叫 Service 執行更新邏輯，回傳的是 bool
            bool isSuccess = await _context.Update(vm);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "商品資料已成功更新！";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "更新失敗");
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PDelete(int id)
        {
            // 呼叫 Service，它會自動判斷要 HardDeleted 還是 SoftDeleted
            var result = await _context.Delete(id);

            // 不論是哪種刪除，對管理員來說都是「這筆資料不見了」
            if (result == "NotFound")
            {
                TempData["ErrorMessage"] = "找不到該行程。";
            }
            else
            {
                // 統一給成功訊息即可，或者根據 result 給不同提示
                TempData["SuccessMessage"] = "行程已成功移除。";
            }

            // 刪完後回到列表頁，因為列表頁有 Where 過濾，那筆資料會自動消失
            return RedirectToAction(nameof(Index));
        }
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
        public async Task<IActionResult> GetResourceDetail(string type, int id)
        {
            // 💡 呼叫 Service 裡的邏輯
            var detail = await _item.GetResourceDetailAsync(type, id);

            // 如果沒抓到資料，回傳空物件避免前端噴錯
            if (detail == null) return Json(new { description = "", images = new List<string>() });

            return Json(detail);
        }
        [HttpGet]
        public async Task<IActionResult> CreateItem(int id)
        { 
         var vm=await _item.PrepareViewModel(id);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemCreate(ViewModelTripItineraryItems vm)
        {
            if (ModelState.IsValid)
            {
                // 💡 呼叫 Service 的 ICreate 方法進行複雜的存檔邏輯
                // 這包含：存圖片檔案、寫入 Resources 表、寫入 TripItineraryItems 表
                bool isSuccess = await _item.ICreate(vm);

                if (isSuccess)
                {
                    return RedirectToAction(nameof(ItemIndex), new { id = vm.TripProductId });
                }
                ModelState.AddModelError("", "儲存失敗，請檢查資料。");
            }

            // 如果失敗，重新準備下拉選單資料再回傳 View
            var reloadVm = await _item.PrepareViewModel(vm.TripProductId);
            return View(reloadVm);
        }

    }
}
