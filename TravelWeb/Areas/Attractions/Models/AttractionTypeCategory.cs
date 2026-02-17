using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionTypeCategory
{
    public int AttractionTypeId { get; set; }

    public string AttractionTypeName { get; set; } = null!;
}
