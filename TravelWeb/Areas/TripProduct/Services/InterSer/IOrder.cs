using TravelWeb.Areas.TripProduct.Models.ViewModels;

namespace TravelWeb.Areas.TripProduct.Services.InterSer
{
    public interface IOrder
    {
        // 1. 獲取訂單列表 (包含狀態燈計算、行程日期、商品摘要)
        // 這對應你首頁的表格，回傳你剛定義的 OrderIndexViewModel
        Task<IEnumerable<OrderIndexViewModel>> GetOrderIndexListAsync();

        // 2. 獲取頂部統計數據 (看板數據)
        // 回傳：今日出團數、未付款數、待處理數
        Task<OrderSummaryViewModel> GetOrderBoardSummaryAsync();

        // 3. 獲取單筆訂單詳情 (進 Details 頁面看票種明細)
        Task<OrderDetailViewModel> GetOrderDetailAsync(int orderId);

        // 4. 商家操作：確認訂單 / 修改狀態
        // 例如：將狀態從「待處理」改為「已完成」
        Task<bool> UpdateStatusAsync(int orderId, string status);

        // 5. 商家操作：核銷票券
        Task<bool> VerifyOrderAsync(int orderId);


    }
}
