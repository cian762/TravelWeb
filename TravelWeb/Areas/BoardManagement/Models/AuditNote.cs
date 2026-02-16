using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class AuditNote
{
    public int TargetId { get; set; }

    public string? Note { get; set; }
}
