namespace TravelWeb.Areas.TripProduct.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        // 訂單基礎資訊 (繼承 Index 的部分欄位或是重複定義)
        public int OrderId { get; set; }
        public string ?OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string ?OrderStatus { get; set; }
        public string ?PaymentStatus { get; set; }

        // 顧客詳細資訊
        public string ?CustomerName { get; set; }
        public string ?ContactPhone { get; set; }
        public string ?ContactEmail { get; set; }
        public string ?CustomerNote { get; set; }

        // 商品與票種明細 (巢狀結構)
        public List<OrderItemDetailViewModel> Items { get; set; } = new();

    }
    public class OrderItemDetailViewModel
    {
        public string ?ProductName { get; set; } // Snapshot
        public List<TicketDetailViewModel> Tickets { get; set; } = new();
    }

    public class TicketDetailViewModel
    {
        public string ?TicketName { get; set; } // Snapshot
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}
