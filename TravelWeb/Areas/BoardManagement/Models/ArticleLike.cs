using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class ArticleLike
{
    public int? ArticleId { get; set; }

    public int? UserId { get; set; }

    public virtual Article? Article { get; set; }
}
