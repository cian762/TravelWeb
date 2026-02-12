using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ItineraryComparison
{
    public int ComparisonId { get; set; }

    public int OriginalVersionId { get; set; }

    public int ComparedVersionId { get; set; }

    public DateTime? ComparisonTime { get; set; }

    public virtual ItineraryVersion ComparedVersion { get; set; } = null!;

    public virtual ItineraryVersion OriginalVersion { get; set; } = null!;
}
