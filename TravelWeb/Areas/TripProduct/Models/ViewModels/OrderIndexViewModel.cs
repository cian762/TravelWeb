namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public class OrderIndexViewModel
    {
        // --- 訂單基礎資訊 (來自 Orders) ---
        public int OrderId { get; set; }
        public string ?OrderNumber { get; set; } // 顯示用編號，如：ORD-20260228-001
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string ?OrderStatus { get; set; } // 訂單狀態：待處理、已完成、已取消

        // --- 顧客資訊 (來自 Member_Information) ---
        public string ?CustomerName { get; set; }
        public string ?ContactPhone { get; set; }

        // --- 商品摘要 (來自 OrderItems) ---
        // 首頁通常只顯示第一項商品名稱 + 「等 N 件」
        public string ?ProductSummary { get; set; }
        public int TotalItemCount { get; set; }

        // --- 金流摘要 (核心：從 PaymentTransactions 聚合而來) ---
        public string ?PaymentStatus { get; set; }   // 付款狀態：已付款、未付款、授權失敗、退款中
        public string ?PaymentMethodName { get; set; } // 支付方式：LinePay, 信用卡, ATM
        public DateTime? PaidAt { get; set; }       // 實際付款時間

        // --- 預警提醒 ---
        public bool IsAmountMismatch { get; set; }  // 應付與實付是否不符 (對帳異常警告)
        public bool HasCustomerNote { get; set; }   // 是否有客戶備註 (顯示小圖示)
    }
}
