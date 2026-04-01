using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models;

[Table("Member_Information", Schema = "Member")]
public partial class MemberInformation
{
    [ValidateNever]
    public string MemberId { get; set; } = null!;
    [ValidateNever]
    public string MemberCode { get; set; } = null!;

    [ForeignKey("MemberCode")] // 指定 MemberCode 為外鍵
    [ValidateNever]
    public virtual MemberList? MemberList { get; set; }

    public string? Name { get; set; }

    public byte? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }
    [ValidateNever]
    public string? AvatarUrl { get; set; }

    [ValidateNever]
    public string? Status { get; set; }

    public virtual ICollection<MemberComplaint> MemberComplaints { get; set; } = new List<MemberComplaint>();

    public virtual ICollection<MemberInformation> Followeds { get; set; } = new List<MemberInformation>();

    public virtual ICollection<MemberInformation> Followers { get; set; } = new List<MemberInformation>();
}
