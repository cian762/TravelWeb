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
    public string? Description { get; set; }    // 景點介紹文字（景點特色 Tab）
    public string? ActivityIntro { get; set; }  // 活動介紹文字（售票區 Tab 下方）
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; }

    public virtual TagsRegion? Region { get; set; }
    public virtual ICollection<AttractionProduct> AttractionProducts { get; set; } = new List<AttractionProduct>();
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    public virtual ICollection<AttractionTypeMapping> AttractionTypeMappings { get; set; } = new List<AttractionTypeMapping>();
}