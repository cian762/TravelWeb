using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Activity.Models.EFModel;

public partial class TicketCategory
{
    public int TicketCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<AcitivityTicket> AcitivityTickets { get; set; } = new List<AcitivityTicket>();
}
