using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models;

public partial class MemberList
{
    [Key]
    public string MemberCode { get; set; } = null!;

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();

    public virtual ICollection<ComplaintRecord> ComplaintRecords { get; set; } = new List<ComplaintRecord>();

    public virtual ICollection<LogInRecord> LogInRecords { get; set; } = new List<LogInRecord>();

    public virtual ICollection<MemberInformation> MemberInformations { get; set; } = new List<MemberInformation>();
}
