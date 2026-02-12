using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ActivityEditLog
{
    public int LogId { get; set; }

    public int? ActivityId { get; set; }

    public int? AdminId { get; set; }

    public string? Action { get; set; }

    public DateTime? EditDate { get; set; }

    public string? OriginalData { get; set; }

    public string? CurrentData { get; set; }

    public virtual Activity? Activity { get; set; }
}
