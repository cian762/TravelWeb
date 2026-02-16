using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class ArticleFolder
{
    public string UserId { get; set; } = null!;

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;
}
