using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<AttractionProduct> Products { get; set; } = new List<AttractionProduct>();
}
