using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class Aianalysis
{
    public int AnalysisId { get; set; }

    public int VersionId { get; set; }

    public decimal? FeasibilityScore { get; set; }

    public decimal? PaceBalanceScore { get; set; }

    public decimal? FatigueIndex { get; set; }

    public DateTime? AnalysisTime { get; set; }

    public virtual ItineraryVersion Version { get; set; } = null!;
}
