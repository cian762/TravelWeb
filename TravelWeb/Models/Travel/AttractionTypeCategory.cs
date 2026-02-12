using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class AttractionTypeCategory
{
    public int AttractionTypeId { get; set; }

    public string AttractionTypeName { get; set; } = null!;
}
