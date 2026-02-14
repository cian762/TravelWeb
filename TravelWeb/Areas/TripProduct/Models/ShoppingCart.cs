using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.TripProduct.Models;

public partial class ShoppingCart
{
    public int CartId { get; set; }

    public int? TicketCategoryId { get; set; }

    public string MemberId { get; set; } = null!;

    public string? ProductCode { get; set; }

    public int? Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual MemberInformation Member { get; set; } = null!;

    public virtual AttractionProduct? ProductCode1 { get; set; }

    public virtual TripSchedule? ProductCode2 { get; set; }

    public virtual AcitivityTicket? ProductCodeNavigation { get; set; }

    public virtual TicketCategory? TicketCategory { get; set; }
}
