using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TravelWeb.Models;

public partial class Administrator
{
    [DisplayName("管理員ID")]
    public string AdminId { get; set; } = null!;

    [DisplayName("信箱")]
    public string? Email { get; set; }

    [DisplayName("姓名")]
    public string? Name { get; set; }

    [DisplayName("電話")]
    public string? Phone { get; set; }

    [DisplayName("密碼")]
    public string? PasswordHash { get; set; }

    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();

    public virtual ICollection<ComplaintRecord> ComplaintRecords { get; set; } = new List<ComplaintRecord>();
}
