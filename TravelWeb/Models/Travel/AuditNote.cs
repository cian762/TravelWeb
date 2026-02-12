using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class AuditNote
{
    public int TargetId { get; set; }

    public string? Note { get; set; }
}
