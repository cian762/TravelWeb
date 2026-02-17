using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class Post
{
    public int ArticleId { get; set; }

    public string? Contents { get; set; }

    public string? Photo { get; set; }

    public int? RegionId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
