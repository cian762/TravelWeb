using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ItineraryVersion
{
    public int VersionId { get; set; }

    public int ItineraryId { get; set; }

    public int? VersionNumber { get; set; }

    public int? Creator { get; set; }

    public string? Source { get; set; }

    public string? VersionRemark { get; set; }

    public DateTime? CreateTime { get; set; }

    public string? CurrentUsageStatus { get; set; }

    public virtual ICollection<Aianalysis> Aianalyses { get; set; } = new List<Aianalysis>();

    public virtual ICollection<AigenerationError> AigenerationErrors { get; set; } = new List<AigenerationError>();

    public virtual Itinerary Itinerary { get; set; } = null!;

    public virtual ICollection<ItineraryComparison> ItineraryComparisonComparedVersions { get; set; } = new List<ItineraryComparison>();

    public virtual ICollection<ItineraryComparison> ItineraryComparisonOriginalVersions { get; set; } = new List<ItineraryComparison>();

    public virtual ICollection<ItineraryItem> ItineraryItems { get; set; } = new List<ItineraryItem>();
}
