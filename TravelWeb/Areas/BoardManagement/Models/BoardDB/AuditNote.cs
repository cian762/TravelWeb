using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class AuditNote
{
    [Key]
    public int TargetId { get; set; }

    public string? Note { get; set; }
}
