using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Activity.Models.EFModel;

public partial class TagsActivityType
{
    public int TypeId { get; set; }

    public string ActivityType { get; set; } = null!;

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
