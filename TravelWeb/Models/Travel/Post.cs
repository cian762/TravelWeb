using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class Post
{
    public int ArticleId { get; set; }

    public string? Contents { get; set; }

    public string? Photo { get; set; }

    public int? RegionId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual TagsRegion? Region { get; set; }
}
