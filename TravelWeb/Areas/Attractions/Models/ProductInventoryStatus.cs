using System;
namespace TravelWeb.Areas.Attractions.Models;

public partial class ProductInventoryStatus
{
    public int ProductId { get; set; }
    public string? InventoryMode { get; set; }
    public int? DailyLimit { get; set; }
    public int? SoldQuantity { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    public virtual AttractionProduct Product { get; set; } = null!;
}