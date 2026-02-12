using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ActivityAnalytic
{
    public int ActivityId { get; set; }

    public int? TotalClicks { get; set; }

    public int? TotalFavorites { get; set; }

    public int? TotalPurchases { get; set; }

    public decimal? Rating { get; set; }

    public virtual Activity? Activity { get; set; }
}
