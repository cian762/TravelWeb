using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // 記得引
namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionProductFavorite
{
    [Key]
    public int FavoriteId { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AttractionProduct Product { get; set; } = null!;
}
