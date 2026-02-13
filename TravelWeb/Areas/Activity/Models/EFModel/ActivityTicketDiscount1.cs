using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Activity.Models.EFModel;

public partial class ActivityTicketDiscount1
{
    public string ProductCode { get; set; } = null!;

    public int DiscountId { get; set; }

    public virtual ActivityTicketDiscount Discount { get; set; } = null!;
}
