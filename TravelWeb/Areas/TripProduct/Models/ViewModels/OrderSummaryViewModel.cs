namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{

    //商家首頁分析數據的配套 
    public class OrderSummaryViewModel
    {

        public int TodayTripsCount { get; set; }      // 今日出團
        public int UnpaidCount { get; set; }          // 未處理 (對應你的 UnpaidCount)
        public int PendingConfirmationCount { get; set; } // 待確認 (對應你的 PendingConfirmationCount)
    }
}
