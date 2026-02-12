using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ItineraryItem
{
    public int ItemId { get; set; }

    public int? AttractionId { get; set; }

    public int VersionId { get; set; }

    public int? DayNumber { get; set; }

    public string? ContentDescription { get; set; }

    public int? SortOrder { get; set; }

    public string? ActivityId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual Attraction? Attraction { get; set; }

    public virtual ItineraryVersion Version { get; set; } = null!;
}
