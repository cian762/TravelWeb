using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class AttractionProductFavorite
{
    public int FavoriteId { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AttractionProduct Product { get; set; } = null!;

    public virtual MemberInformation User { get; set; } = null!;
}
