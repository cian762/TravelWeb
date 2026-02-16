using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class JournalPage
{
    public int ArticleId { get; set; }

    public int Date { get; set; }

    public int? RegionId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
