using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models;

[Table("Administrator", Schema = "Member")]
public partial class Administrator
{
    public string AdminId { get; set; } = null!;

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Authorization> Authorizations { get; set; } = new List<Authorization>();

    public virtual ICollection<ComplaintRecord> ComplaintRecords { get; set; } = new List<ComplaintRecord>();
}
