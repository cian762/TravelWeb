using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class TagsActivityType
{
    public int TypeId { get; set; }

    public string? ActivityType { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
