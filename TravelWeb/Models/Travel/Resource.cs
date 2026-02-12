using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class Resource
{
    public int ResourceId { get; set; }

    public string? ResourceName { get; set; }

    public string? ShortDescription { get; set; }

    public string? MainImage { get; set; }

    public string? DefaultDescription { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual ICollection<TripItineraryItem> TripItineraryItems { get; set; } = new List<TripItineraryItem>();
}
