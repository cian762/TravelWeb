using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionProductDetail
{
    public int ProductId { get; set; }
    public string? ContentDetails { get; set; }
    public string? Notes { get; set; }              // 內部備註（不對外顯示）
    public string? UsageInstructions { get; set; }
    public string? Includes { get; set; }
    public string? Excludes { get; set; }
    public string? Eligibility { get; set; }
    public string? CancelPolicy { get; set; }
    public string? ValidityNote { get; set; }       // 有效期說明文字（對外顯示）
    public DateTime? LastUpdatedAt { get; set; }

    public virtual AttractionProduct? Product { get; set; }
}