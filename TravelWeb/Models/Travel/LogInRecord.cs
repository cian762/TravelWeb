using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class LogInRecord
{
    public string? MemberCode { get; set; }

    public DateTime? LoginAt { get; set; }

    public int LoginRecordId { get; set; }

    public virtual MemberList? MemberCodeNavigation { get; set; }
}
