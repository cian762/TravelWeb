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

    // 選擇性加入：若需要顯示會員資訊
    // 注意：Member 是在不同 DbContext，加了這個導覽屬性
    // 需要在 Context 裡特別處理，否則建議不加
    // public virtual MemberInformation Member { get; set; } = null!;
}
