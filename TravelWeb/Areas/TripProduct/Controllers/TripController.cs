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
        private readonly ITripSchedules _trip;
        public TripController(ITripproducts context, ITripItineraryItem item, ITripSchedules trip)
        { 
         _context = context;
            _item = item;
            _trip = trip;
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
            if (ModelState.IsValid)
            {
                // 💡 呼叫修改後的 Service，拿到新生成的產品 ID
                int newProductId = await _context.Create(products);

                if (newProductId > 0)
                {
                    TempData["SuccessMessage"] = "旅遊產品建立成功！現在開始編排行程細項。";
                    TempData.Keep("SuccessMessage");

                    // 💡 直接跳轉到 Trip 控制器的 CreateItem，並帶入 ID
                    return RedirectToAction("CreateItem", "Trip", new { id = newProductId });
                }
            }

            // 失敗則停留在原頁面
            products = await _context.GetCreateViewModelAsync();
            return View(products);

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
            // 1. 呼叫 Service 抓取現有的行程資料
            var q = await _item.IGetAny(id);

            // 2. 💡 關鍵判斷：如果有資料，就正常顯示列表頁
            if (q != null && q.Any())
            {
                ViewBag.ProductId = id;
                ViewBag.ProductName = "行程編排總覽"; 

                return View(q);
            }

            // 3. 如果完全沒東西，才導向新增頁面，避免重複點擊
            return RedirectToAction("CreateItem", new { id = id });
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
            // 1. 呼叫 Service 計算下一個建議的天數與排序
            var vm = await _item.PrepareViewModel(id);

            // 💡 關鍵檢查：如果 Service 判定全滿，會回傳 DayNumber = 0
            if (vm.DayNumber == 0)
            {
                // 儲存警告訊息，供前端彈窗顯示
                TempData["ErrorMessage"] = "該行程的所有天數與排序已全數編排完成，無法再新增細項。";

                // 直接踢回列表頁面，阻止進入新增畫面
                return RedirectToAction("ItemIndex", new { id = id });
            }

            // 2. 沒滿才繼續執行：抓取現有行程顯示在時間軸
            ViewBag.ExistingItems = await _item.IGetAny(id);

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(ViewModelTripItineraryItems vm)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess;

                // 1. 判斷是「更新」還是「新增」
                if (vm.ItineraryItemId > 0)
                {
                    isSuccess = await _item.IUpdate(vm);
                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "行程細項已成功更新！";
                        return RedirectToAction("ItemIndex", new { id = vm.TripProductId });
                    }
                }
                else
                {
                    // 💡 修正：這裡呼叫一次就好
                    isSuccess = await _item.ICreate(vm);
                    if (isSuccess)
                    {
                        // 💡 關鍵：刪掉原本檢查 DayNumber >= MaxDays 的 if
                        // 直接導回 CreateItem，讓 PrepareViewModel 幫你跳到下一格
                        TempData["SuccessMessage"] = $"第 {vm.DayNumber} 天序號 {vm.SortOrder} 已存檔。";
                        return RedirectToAction("CreateItem", new { id = vm.TripProductId });
                    }
                }
                ModelState.AddModelError("", "儲存失敗，請檢查資料。");
            }

            // --- 💡 關鍵修改：驗證失敗時的處理 ---

            // A. 無論是新增或編輯，都需要重新準備下拉選單資料
            var template = await _item.PrepareViewModel(vm.TripProductId);
            vm.AttractionList = template.AttractionList;
            vm.ActivityList = template.ActivityList;
            vm.MaxDays = template.MaxDays;

            // B. 根據 ID 判斷該導向哪一個 View
            if (vm.ItineraryItemId > 0)
            {
                // 💡 編輯失敗：回 EditItem.cshtml
                // 不需要右邊的進度表，所以不用抓 IGetAny
                return View("ItemUpData", vm);
            }
            else
            {
                // 💡 新增失敗：回 CreateItem.cshtml
                ViewBag.ExistingItems = await _item.IGetAny(vm.TripProductId);
                return View("CreateItem", vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ItemDelete(int id)
        {
            // 1. 抓取資料庫資料 (用於獲取 ProductId)
            var item = await _item.GetById(id);
            if (item == null) return Json(new { success = false, message = "找不到該細項。" });

            int productId = item.TripProductId;

            // 2. 執行刪除邏輯 (含圖片、資源)
            var success = await _item.IDelete(id);

            // 💡 關鍵：必須回傳 JSON，前端 Ajax 才能接收成功訊號
            if (success)
            {
                return Json(new { success = true, productId = productId });
            }

            return Json(new { success = false, message = "移除失敗。" });

        }
        [HttpGet]
        public async Task<IActionResult> IUpdataItem(int id)
        {
            // 💡 1. 呼叫服務層，取得填好舊資料與選單(AttractionList等)的 ViewModel
            var vm = await _item.GetEditViewModel(id);

            if (vm == null)
            {
                return NotFound();
            }

            // 💡 2. 為了讓頁面正確顯示標題，可以補上這些 ViewBag
            ViewBag.ProductId = vm.TripProductId;

            // 💡 3. 返回原本的 CreateItem 視圖
            return View("ItemUpData", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IUpdataItem(ViewModelTripItineraryItems vm)
        {
            if (ModelState.IsValid)
            {
                // 呼叫 Service 執行更新資料庫的動作
                bool isSuccess = await _item.IUpdate(vm);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "行程細項已成功更新！";
                    // 存檔成功後導回列表頁
                    return RedirectToAction("ItemIndex", new { id = vm.TripProductId });
                }
            }

            // 驗證失敗，重新加載下拉選單並回傳原頁面
            var template = await _item.PrepareViewModel(vm.TripProductId);
            vm.AttractionList = template.AttractionList;
            vm.ActivityList = template.ActivityList;

            return View("ItemUpData", vm);
        }
        //這裡是行程檔期
        [HttpGet]
        public async Task<IActionResult> SchedulesIndex(int id, string filter = "active")
        {
            // 1. 準備畫面需要的基礎 ViewBag
            ViewBag.TripProductId = id;
            ViewBag.Filter = filter;

            // 💡 2. 透過現有的 Service 方法抓取產品名稱
            ViewBag.ProductName = await _trip.GetProductNameAsync(id);

            // 💡 3. 重點：為了讓「新增彈窗」有票種清單可以選，呼叫你的 Prepare 方法
            // 這裡我們直接利用你已經寫好的 PrepareScheduleViewModel
            var createInfo = await _trip.PrepareScheduleViewModel(id);
            // 💡 實務邏輯：預設出發日為「今天 + 45 天」，確保符合退款政策的緩衝期
            var bufferDate = DateTime.Today.AddDays(45);
            ViewBag.DefaultStartDate = bufferDate.ToString("yyyy-MM-dd");

            // 💡 防呆：日期選擇器最小值也應設為 45 天後 (或是你允許緊急接單就設為 Today)
            // 建議設為 Today，但讓預設值跳到 45 天後，給予彈性
            ViewBag.MinDate = DateTime.Today.ToString("yyyy-MM-dd");

            ViewBag.TicketOptions = createInfo.AllTicketCategories;
            ViewBag.DurationDays = createInfo.DurationDays;
            ViewBag.DefaultPrice = createInfo.Price;

            var vmList = await _trip.GetScheduleListAsync(id, filter);
            return View("SchedulesTripIndex", vmList);
        }
        [HttpGet]
        public async Task<IActionResult> CreateSchedule(int tripProductId)
        {
            // 💡 調用你介面中現有的 PrepareScheduleViewModel
            var vm = await _trip.PrepareScheduleViewModel(tripProductId);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(ViewModelTripSchedule vm)
        {
            // 1. 檢查必填欄位有無漏掉
            if (!ModelState.IsValid)
            {
                // 如果驗證失敗，因為是彈窗，建議先導回列表頁並用 TempData 提示錯誤
                TempData["ErrorMessage"] = "資料格式有誤，請檢查後再試！";
                return RedirectToAction("SchedulesIndex", new { id = vm.TripProductId });
            }

            // 2. 呼叫你介面中已經定義好的 CreateSchedule
            // 注意：這裡 vm 會包含你勾選的 SelectedTicketIds
            bool isSuccess = await _trip.CreateSchedule(vm);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = "檔期 " + vm.ProductCode + " 已成功新增！";
            }
            else
            {
                TempData["ErrorMessage"] = "新增失敗！可能是檔期代碼重複，或是系統異常。";
            }

            // 3. 儲存完畢，直接回到該行程的檔期列表
            return RedirectToAction("SchedulesIndex", new { id = vm.TripProductId });
        }
        [HttpGet]
        public async Task<IActionResult> GetNextCode(int tripId, string date)
        {
            if (DateTime.TryParse(date, out DateTime startDate))
            {
                var nextCode = await _trip.GetNextProductCode(tripId, startDate);
                return Json(new { code = nextCode });
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetScheduleData(string productCode)
        {
            // 💡 呼叫 Service 抓出 ViewModel
            var data = await _trip.GetScheduleForEdit(productCode);

            if (data == null) return NotFound();

            // 傳回 JSON 讓前端填入彈窗
            return Json(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSchedule(ViewModelTripSchedule vm)
        {
            // 1. 如果驗證失敗，帶回錯誤訊息並導回列表
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "資料格式錯誤，請重新檢查。";
                return RedirectToAction("SchedulesIndex", new { id = vm.TripProductId });
            }

            // 2. 呼叫 Service 執行修改
            bool isSuccess = await _trip.UpdateSchedule(vm);

            if (isSuccess)
            {
                TempData["SuccessMessage"] = $"檔期 {vm.ProductCode} 修改成功！";
            }
            else
            {
                TempData["ErrorMessage"] = "修改失敗，請稍後再試。";
            }

            // 3. 導回該行程的檔期列表
            return RedirectToAction("SchedulesIndex", new { id = vm.TripProductId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSchedule(string productCode) // 💡 必須與 JS 的 data 鍵名對應
        {

            // 呼叫 Service
            bool isSuccess = await _trip.DeleteSchedule(productCode.Trim());

            if (isSuccess)
            {
                return Json(new { success = true, message = "檔期已成功刪除。" });
            }

            return Json(new { success = false, message = "刪除失敗！可能該檔期已有人報名，或代碼不存在。" });
        }
    }
}
