using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionProductDetail
{
    public int ProductId { get; set; }

    public string? ContentDetails { get; set; }

    public string? Notes { get; set; }

    public string? UsageInstructions { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    // 只留這一個，? 代表允許 null
    public virtual AttractionProduct? Product { get; set; }

    // 確認 MVC 的 AttractionProductDetail.cs 有這些屬性
    public string? Includes { get; set; }
    public string? Excludes { get; set; }
    public string? Eligibility { get; set; }
    public string? CancelPolicy { get; set; }
}
