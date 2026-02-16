using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class ReportLog
{
    public int LogId { get; set; }

    public int TargetId { get; set; }

    public byte TargetType { get; set; }

    public int UserId { get; set; }

    public byte ViolationType { get; set; }

    public string? Reason { get; set; }

    public string? Photo { get; set; }

    public string? Snapshot { get; set; }

    public DateTime CreatedAt { get; set; }

    public byte? ResultType { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte Status { get; set; }
}
