using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class Itinerary
{
    public int ItineraryId { get; set; }

    public string MemberId { get; set; } = null!;

    public string ItineraryName { get; set; } = null!;

    public string? ItineraryImage { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Introduction { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? CurrentStatus { get; set; }

    public virtual ICollection<ActivityMapping> ActivityMappings { get; set; } = new List<ActivityMapping>();

    public virtual ICollection<ItineraryVersion> ItineraryVersions { get; set; } = new List<ItineraryVersion>();

    public virtual MemberInformation Member { get; set; } = null!;
}
