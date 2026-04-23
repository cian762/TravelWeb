using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models;

[Table("Authorization", Schema = "Member")]
public partial class Authorization
{
    public string AdminId { get; set; } = null!;

    public string? Permission { get; set; }

    public string MemberCode { get; set; } = null!;

    public string? Remark { get; set; }

    public DateTime? ExecutedAt { get; set; }

    public int AuthorizationId { get; set; }

    public virtual Administrator Admin { get; set; } = null!;

    public virtual MemberList MemberCodeNavigation { get; set; } = null!;
}
