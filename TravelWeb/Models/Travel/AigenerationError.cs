using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class AigenerationError
{
    public int ErrorId { get; set; }

    public int? VersionId { get; set; }

    public string? ErrorType { get; set; }

    public string? SeverityLevel { get; set; }

    public string? ErrorMessage { get; set; }

    public int? RelatedItemId { get; set; }

    public bool? IsConfirmed { get; set; }

    public DateTime? CreateTime { get; set; }

    public virtual ItineraryVersion? Version { get; set; }
}
