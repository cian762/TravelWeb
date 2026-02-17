using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class ViewArticleHideStatus
{
    [Key]
    public int TargetId { get; set; }

    public int? ReportCount { get; set; }

    public int? SolvedCount { get; set; }

    public int? UnauditedCount { get; set; }

    public string? Note { get; set; }

    public byte? Status { get; set; }

    public bool? IsViolation { get; set; }
}
