using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ArticleLike
{
    public int? ArticleId { get; set; }

    public int? UserId { get; set; }

    public virtual Article? Article { get; set; }
}
