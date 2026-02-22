using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;
using TravelWeb.Areas.TripProduct.Services.InterSer;

namespace TravelWeb.Areas.TripProduct.Services.Implementation
{
    public class STripSchedules : ITripSchedules
    {
        private readonly TripDbContext _trip;
        public STripSchedules (TripDbContext trip) 
        {
            _trip = trip;
      
        }
        //這是新增檔期的方法
        public async Task<bool> CreateSchedule(ViewModelTripSchedule vm)
        {
            try
            {
                var newSchedule = new TripSchedule
                {
                    ProductCode = vm.ProductCode ?? "", // 直接用前端傳來的編號
                    TripProductId = vm.TripProductId,
                    StartDate = DateOnly.FromDateTime(vm.StartDate),
                    EndDate = DateOnly.FromDateTime(vm.EndDate),
                    Price = vm.Price,
                    MaxCapacity = vm.MaxCapacity,
                    SoldQuantity = 0,
                    Status = "開放中"
                };

                var selectedIds = vm.SelectedTicketIds ?? new List<int>();
                if (selectedIds.Any())
                {
                    var tickets = await _trip.TicketCategories
                        .Where(t => selectedIds.Contains(t.TicketCategoryId))
                        .ToListAsync();
                    newSchedule.TicketCategories = tickets;
                }

                _trip.TripSchedules.Add(newSchedule);
                await _trip.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        // 獨立出來的「算編號」邏輯
        public async Task<string> GetNextProductCode(int tripId, DateTime startDate)
        {
            string prefix = $"TP{tripId}-{startDate:MMdd}-";

            var lastEntry = await _trip.TripSchedules
                .Where(s => s.ProductCode.StartsWith(prefix))
                .OrderByDescending(s => s.ProductCode)
                .Select(s => s.ProductCode)
                .FirstOrDefaultAsync();

            int nextNum = 1;
            if (lastEntry != null)
            {
                string suffix = lastEntry.Substring(lastEntry.Length - 2);
                if (int.TryParse(suffix, out int lastNum))
                {
                    nextNum = lastNum + 1;
                }
            }
            return prefix + nextNum.ToString("D2");
        }
        //這是刪除檔期的方法
        public async Task<bool> DeleteSchedule(string productCode)
        {
            try
            {
                // 1. 抓出該檔期，必須包含票種關聯 (TicketCategories)
                var schedule = await _trip.TripSchedules
                    .Include(s => s.TicketCategories) // 💡 關鍵：把關聯表一起拉出來
                    .FirstOrDefaultAsync(s => s.ProductCode == productCode.Trim());

                if (schedule == null) return false;

                // 2. 防呆：有人報名絕對不能刪
                if (schedule.SoldQuantity > 0) return false;

                // 3. 💡 核心修復：先切斷與票種的關聯 (清空中間表)
                // 這會刪除 TripAndTicketRelation 裡對應 ProductCode 的資料
                schedule.TicketCategories.Clear();

                // 4. 執行刪除主檔
                _trip.TripSchedules.Remove(schedule);

                // 5. 統一存檔
                await _trip.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //編輯檔期時的還原畫面方法
        public async Task<ViewModelTripSchedule?> GetScheduleForEdit(string productCode)
        {
            // 💡 A. 抓出該筆檔期實體，並【直接包含 (Include)】關聯的票種
            var schedule = await _trip.TripSchedules
                .AsNoTracking()
                .Include(s => s.TicketCategories) // 👈 關鍵：把隱含多對多關聯的票種一起拉出來
                .FirstOrDefaultAsync(s => s.ProductCode == productCode);

            if (schedule == null) return null; // 找不到就回傳 null

            // 💡 B. 抓取對應的產品主表 (為了拿天數跟名稱)
            var product = await _trip.TripProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TripProductId == schedule.TripProductId);

            // 💡 C. 將資料塞入 ViewModel
            var vm = new ViewModelTripSchedule
            {
                ProductCode = schedule.ProductCode,
                TripProductId = schedule.TripProductId,

                // 處理資料庫裡允許為 Null 的數值
                MaxCapacity = schedule.MaxCapacity ?? 0,
                Price = schedule.Price ?? 0m,

                // Status 直接對應
                Status = schedule.Status ?? "開放中",

                // 處理 DateOnly? 轉 DateTime
                StartDate = schedule.StartDate.HasValue
                    ? schedule.StartDate.Value.ToDateTime(TimeOnly.MinValue)
                    : DateTime.Today,

                EndDate = schedule.EndDate.HasValue
                    ? schedule.EndDate.Value.ToDateTime(TimeOnly.MinValue)
                    : DateTime.Today,

                ProductName = product?.ProductName,
                DurationDays = product?.DurationDays ?? 1,

                // 💡 D. 完美解法：直接從剛才 Include 進來的 TicketCategories 抓取 ID
                SelectedTicketIds = schedule.TicketCategories != null
                    ? schedule.TicketCategories.Select(tc => tc.TicketCategoryId).ToList()
                    : new List<int>()
            };

            // 💡 E. 依然要抓取所有票種，不然畫面畫不出 Checkbox
            vm.AllTicketCategories = await _trip.TicketCategories
                .AsNoTracking()
                .Select(t => new SelectListItem
                {
                    Value = t.TicketCategoryId.ToString(),
                    Text = t.CategoryName
                }).ToListAsync();

            return vm;
        }

        //列表頁抓取行程所有檔期的方法
        public async Task<List<ViewModelTripSchedule>> GetScheduleListAsync(int tripProductId, string filter)
        {
            // 1. 建立基礎查詢
            var query = _trip.TripSchedules
                .AsNoTracking()
                .Where(s => s.TripProductId == tripProductId);

            // 2. 在資料庫層級直接做 Filter 過濾 (效能最佳)
            var today = DateOnly.FromDateTime(DateTime.Today); // 配合你的 DateOnly 型別

            if (filter == "active")
            {
                query = query.Where(s => s.EndDate >= today);
            }
            else if (filter == "past")
            {
                query = query.Where(s => s.EndDate < today);
            }

            // 3. 排序並撈出實體資料
            var rawData = await query.OrderByDescending(s => s.StartDate).ToListAsync();

            // 4. 轉換為 ViewModel
            return rawData.Select(s => new ViewModelTripSchedule
            {
                ProductCode = s.ProductCode,
                TripProductId = s.TripProductId,
                Price = s.Price ?? 0m,
                MaxCapacity = s.MaxCapacity ?? 0,
                Status = s.Status ?? "開放中",
                SoldQuantity = s.SoldQuantity ?? 0, // 👈 為前台進度條準備的欄位

                // 處理 DateOnly? 轉 DateTime
                StartDate = s.StartDate.HasValue ? s.StartDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today,
                EndDate = s.EndDate.HasValue ? s.EndDate.Value.ToDateTime(TimeOnly.MinValue) : DateTime.Today
            }).ToList();
        }
        //新增檔期時的畫面資料的方法
        public async Task<ViewModelTripSchedule> PrepareScheduleViewModel(int tripProductId)
        {
            var vm = new ViewModelTripSchedule { TripProductId = tripProductId };

            // 💡 A. 抓取行程主表的預設資訊 (確保 UI 能自動算天數)
            var product = await _trip.TripProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TripProductId == tripProductId);

            if (product != null)
            {
                vm.ProductName = product.ProductName;
                vm.DurationDays = product.DurationDays ?? 1;
                vm.Price = product.DisplayPrice ?? 0; // 預設先帶入主表展示價，省去手打
            }

            // 💡 B. 預設出發與回程日期 (避免畫面上空空的)
            vm.StartDate = DateTime.Today.AddDays(7); // 預設下週出發
            vm.EndDate = vm.StartDate.AddDays(vm.DurationDays - 1); // 自動推算回程日

            // 💡 C. 抓取所有票種，供畫面渲染 Checkbox
            vm.AllTicketCategories = await _trip.TicketCategories
                .AsNoTracking()
                .Select(t => new SelectListItem
                {
                    Value = t.TicketCategoryId.ToString(),
                    Text = t.CategoryName
                }).ToListAsync();

            return vm;
        }
        //修改檔期的方法
        public async Task<bool> UpdateSchedule(ViewModelTripSchedule vm)
        {
            try
            {
                // 1. 找出原始資料，必須 Include 票種關聯才能進行更新
                var existingSchedule = await _trip.TripSchedules
                    .Include(s => s.TicketCategories)
                    .FirstOrDefaultAsync(s => s.ProductCode == vm.ProductCode);

                if (existingSchedule == null) return false;

                // 2. 更新基本欄位 (將 DateTime 轉回資料庫用的 DateOnly)
                existingSchedule.StartDate = DateOnly.FromDateTime(vm.StartDate);
                existingSchedule.EndDate = DateOnly.FromDateTime(vm.EndDate);
                existingSchedule.Price = vm.Price;
                existingSchedule.MaxCapacity = vm.MaxCapacity;
                existingSchedule.Status = vm.Status ?? "開放中";

                // 3. 更新「多對多」票種關聯
                // 先清除該檔期舊有的所有票種連結
                existingSchedule.TicketCategories.Clear();

                var selectedIds = vm.SelectedTicketIds ?? new List<int>();
                if (selectedIds.Any())
                {
                    // 抓出畫面上勾選的新票種實體
                    var newTickets = await _trip.TicketCategories
                        .Where(t => selectedIds.Contains(t.TicketCategoryId))
                        .ToListAsync();

                    // 重新加入關聯
                    foreach (var ticket in newTickets)
                    {
                        existingSchedule.TicketCategories.Add(ticket);
                    }
                }

                // 4. 執行資料庫存檔
                await _trip.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //查詢行程商品名稱的方法
        public async Task<string> GetProductNameAsync(int tripProductId)
        {
            var product = await _trip.TripProducts.FindAsync(tripProductId);
            return product?.ProductName ?? "未知行程";
        }
    }
}
