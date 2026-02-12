using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ActivityTicketDetail
{
    public string ProductCode { get; set; } = null!;

    public int? ActivityId { get; set; }

    public string? ProdcutDescription { get; set; }

    public string? TermsOfService { get; set; }

    public virtual AcitivityTicket? AcitivityTicket { get; set; }

    public virtual Activity? Activity { get; set; }
}
