namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public class OrderIndexViewModel
    {
        // --- 核心：狀態燈與出團日 ---
        public string ?StatusLightClass { get; set; }  // 🔴/🟡/🟢 的 CSS 類別
        public DateTime? TravelDate { get; set; }     // 行程日期 (商家最在意！)

        // --- 訂單基礎資訊 ---
        public int OrderId { get; set; }
        public string? OrderNumber { get; set; }      // 格式化編號
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }

        // --- 顧客與商品摘要 ---
        public string? CustomerName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ProductSummary { get; set; }   // 台北一日遊 等 2 件
        public int TotalItemCount { get; set; }

        // --- 金流摘要 ---
        public string? PaymentStatus { get; set; }
        public string? PaymentMethodName { get; set; }

        // --- 預警提醒 ---
        public bool IsAmountMismatch { get; set; }
        public bool HasCustomerNote { get; set; }
    }
}
