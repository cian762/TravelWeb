namespace TravelWeb.Areas.Attractions.Models
{
    public class InventoryViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? InventoryMode { get; set; }  // UNLIMITED / DAILY / TOTAL
        public int? DailyLimit { get; set; }
        public int? SoldQuantity { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public int RemainingStock { get; set; }
    }
}
