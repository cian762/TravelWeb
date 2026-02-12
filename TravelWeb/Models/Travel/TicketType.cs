using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class TicketType
{
    public string TicketTypeCode { get; set; } = null!;

    public string TicketTypeName { get; set; } = null!;

    public int? SortOrder { get; set; }
}
