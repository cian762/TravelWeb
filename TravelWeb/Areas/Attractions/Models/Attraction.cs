using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class Attraction
{
    public int AttractionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public int RegionId { get; set; }

    public string? AreaId { get; set; }

    public int? ApprovalStatus { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? OpendataId { get; set; }

    public string? Phone { get; set; }

    public string? Website { get; set; }

    public string? BusinessHours { get; set; }

    public string? GooglePlaceId { get; set; }

    public string? ClosedDaysNote { get; set; }

    public string? TransportInfo { get; set; }

    public DateTime CreatedAt { get; set; }

    // 這行是告訴系統：Attraction 裡面有一個屬性連向 TaqsRegion 表
    // 這裡的名稱必須叫 Region，這樣你的 .Include(a => a.Region) 才會通
    public virtual TagsRegion? Region { get; set; }
    public virtual ICollection<AttractionProduct> AttractionProducts { get; set; } = new List<AttractionProduct>();
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    // 讓景點知道它擁有很多個類別關聯
    public virtual ICollection<AttractionTypeMapping> AttractionTypeMappings { get; set; } = new List<AttractionTypeMapping>();
}
