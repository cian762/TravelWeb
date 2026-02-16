using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class PostPhoto
{
    public int ArticleId { get; set; }

    public string? Photo { get; set; }

    public virtual Article Article { get; set; } = null!;
}
