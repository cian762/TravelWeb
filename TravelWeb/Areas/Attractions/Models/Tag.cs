using System;
using System.Collections.Generic;

namespace TravelWeb.Models; // 建議統一命名空間

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    // 這是對接中間表的集合
    public virtual ICollection<AttractionProductTag> AttractionProductTags { get; set; } = new HashSet<AttractionProductTag>();
}