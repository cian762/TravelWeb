using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class Journal
{
    public int ArticleId { get; set; }

    public byte? CoverId { get; set; }

    public DateOnly? ScheduledDate { get; set; }

    public int? TemplateId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
