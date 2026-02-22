using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Models.ViewModels;

namespace TravelWeb.Areas.TripProduct.Services.InterSer
{
    public interface ITripSchedules
    {
        // --- 查詢與初始化 ---
        Task<ViewModelTripSchedule> PrepareScheduleViewModel(int tripProductId);
        Task<List<ViewModelTripSchedule>> GetScheduleListAsync(int tripProductId, string filter);
        Task<ViewModelTripSchedule?> GetScheduleForEdit(string productCode);
        Task<string> GetProductNameAsync(int tripProductId);
        Task<string> GetNextProductCode(int tripId, DateTime startDate);

        // --- 新增 ---
        Task<bool> CreateSchedule(ViewModelTripSchedule vm);

        // --- 修改 ---
        Task<bool> UpdateSchedule(ViewModelTripSchedule vm);

        // --- 刪除與狀態管理 ---
        Task<bool> DeleteSchedule(string productCode);
    
    }
}
