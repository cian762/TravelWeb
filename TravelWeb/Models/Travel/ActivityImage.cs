using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ActivityImage
{
    public int ImageSetId { get; set; }

    public int? ActivityId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Activity? Activity { get; set; }
}
