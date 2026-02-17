using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class ArticleSource
{
    public int ArticleId { get; set; }

    public int Source { get; set; }

    public virtual Article Article { get; set; } = null!;
}
