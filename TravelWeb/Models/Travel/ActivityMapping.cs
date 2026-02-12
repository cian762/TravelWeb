using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ActivityMapping
{
    public int MappingId { get; set; }

    public string ActivityId { get; set; } = null!;

    public int ItineraryId { get; set; }

    public DateTime? CreateTime { get; set; }

    public virtual Itinerary Itinerary { get; set; } = null!;
}
