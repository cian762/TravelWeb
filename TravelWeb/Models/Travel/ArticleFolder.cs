using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class ArticleFolder
{
    public string UserId { get; set; } = null!;

    public int ArticleId { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual MemberInformation User { get; set; } = null!;
}
