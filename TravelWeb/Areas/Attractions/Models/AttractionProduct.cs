using System;
using System.Collections.Generic;
using TravelWeb.Models;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionProduct
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = null!;
    public int AttractionId { get; set; }
    public string Title { get; set; } = null!;
    public string? Status { get; set; }
    public int? PolicyId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal? Price { get; set; }
    public int? MaxPurchaseQuantity { get; set; }
    public int? IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? TicketTypeCode { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int? ValidityDays { get; set; }

    public virtual TicketType? TicketType { get; set; }
    public virtual Attraction? Attraction { get; set; } = null!;
    public virtual AttractionProductDetail? AttractionProductDetail { get; set; }
    public virtual ICollection<AttractionProductFavorite> AttractionProductFavorites { get; set; } = new List<AttractionProductFavorite>();
    public virtual ICollection<AttractionProductImage> AttractionProductImages { get; set; } = new List<AttractionProductImage>();
    public virtual ICollection<StockInRecord> StockInRecords { get; set; } = new List<StockInRecord>();
    public virtual ICollection<AttractionProductTag> AttractionProductTags { get; set; } = new HashSet<AttractionProductTag>();
}