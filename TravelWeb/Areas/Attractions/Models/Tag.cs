using System;
using System.Collections.Generic;
using TravelWeb.Models;

namespace TravelWeb.Areas.Attractions.Models;

public partial class Tag
{
    public int TagId { get; set; }
    public string TagName { get; set; } = null!;
    public string? Description { get; set; }        // 標籤說明文字（顯示在 drawer）

    public virtual ICollection<AttractionProductTag> AttractionProductTags { get; set; }
        = new List<AttractionProductTag>();
}