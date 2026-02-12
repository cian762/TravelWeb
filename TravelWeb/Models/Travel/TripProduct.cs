using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class TripProduct
{
    public int TripProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public int? DurationDays { get; set; }

    public string? CoverImage { get; set; }

    public decimal? DisplayPrice { get; set; }

    public int? ClickTimes { get; set; }

    public string? Status { get; set; }

    public int? PolicyId { get; set; }

    public int? RegionId { get; set; }

    public virtual CancellationPolicy? Policy { get; set; }

    public virtual TagsRegion? Region { get; set; }

    public virtual ICollection<TripItineraryItem> TripItineraryItems { get; set; } = new List<TripItineraryItem>();

    public virtual ICollection<TripSchedule> TripSchedules { get; set; } = new List<TripSchedule>();
}
