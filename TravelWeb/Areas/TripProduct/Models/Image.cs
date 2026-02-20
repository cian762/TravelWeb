using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.TripProduct.Models;

public partial class Image
{
    public int? AttractionId { get; set; }

    public string? ImagePath { get; set; }

    public virtual Attraction? Attraction { get; set; }
}
