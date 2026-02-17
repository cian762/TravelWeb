using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionTypeMapping
{
    public int? AttractionTypeId { get; set; }

    public int? AttractionId { get; set; }

    public virtual Attraction? Attraction { get; set; }

    public virtual AttractionTypeCategory? AttractionType { get; set; }
}
