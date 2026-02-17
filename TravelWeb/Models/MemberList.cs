using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TravelWeb.Models;

public partial class MemberList
{
    [DisplayName("識別碼")]
    public string MemberCode { get; set; } = null!;

    [DisplayName("郵件")]
    public string? Email { get; set; }

    [DisplayName("密碼")]
    public string? PasswordHash { get; set; }

    [DisplayName("電話")]
    public string? Phone { get; set; }

    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();

    public virtual ICollection<ComplaintRecord> ComplaintRecords { get; set; } = new List<ComplaintRecord>();

    public virtual ICollection<LogInRecord> LogInRecords { get; set; } = new List<LogInRecord>();

    public virtual ICollection<MemberInformation> MemberInformations { get; set; } = new List<MemberInformation>();
}
