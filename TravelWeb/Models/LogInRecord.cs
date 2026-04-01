using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TravelWeb.Models;

[Table("Log_in_record", Schema = "Member")]
public partial class LogInRecord
{
    public string? MemberCode { get; set; }

    public DateTime? LoginAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LoginRecordId { get; set; }

    public virtual MemberList? MemberCodeNavigation { get; set; }
}
