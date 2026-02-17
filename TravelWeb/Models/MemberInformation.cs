using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TravelWeb.Models;

public partial class MemberInformation
{
    [DisplayName("id")]
    public string MemberId { get; set; } = null!;

    [DisplayName("會員識別碼")]
    public string MemberCode { get; set; } = null!;

    [DisplayName("姓名")]
    public string? Name { get; set; }

    [DisplayName("姓別")]
    public byte? Gender { get; set; }

    [DisplayName("生日")]
    public DateOnly? BirthDate { get; set; }

    [DisplayName("大頭貼")]
    public string? AvatarUrl { get; set; }

    [DisplayName("狀態")]
    public string? Status { get; set; }

    public virtual Blocked Member { get; set; } = null!;

    public virtual MemberList MemberCodeNavigation { get; set; } = null!;

    public virtual ICollection<MemberComplaint> MemberComplaints { get; set; } = new List<MemberComplaint>();

    public virtual ICollection<MemberInformation> Followeds { get; set; } = new List<MemberInformation>();

    public virtual ICollection<MemberInformation> Followers { get; set; } = new List<MemberInformation>();
}
