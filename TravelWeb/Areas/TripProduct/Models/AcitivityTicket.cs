using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.TripProduct.Models;

public partial class AcitivityTicket
{
    public string ProductCode { get; set; } = null!;

    public string? ProductName { get; set; }

    public int? TicketCategoryId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? CurrentPrice { get; set; }

    public string? ProductLink { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

    public virtual TicketCategory? TicketCategory { get; set; }
}
