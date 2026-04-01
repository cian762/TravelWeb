using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models;

[Table("MemberComplaint", Schema = "Member")]
public partial class MemberComplaint
{
    public string MemberId { get; set; } = null!;

    public string ComplaintId { get; set; } = null!;

    public string? Description { get; set; }

    public string? ReplyEmail { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ComplaintRecord Complaint { get; set; } = null!;

    public virtual MemberInformation Member { get; set; } = null!;
}
