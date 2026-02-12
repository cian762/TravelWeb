using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ArticleSource
{
    public int ArticleId { get; set; }

    public int Source { get; set; }

    public virtual Article Article { get; set; } = null!;
}
