using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class VwAllProductList
{
    public string? ProductName { get; set; }

    public string ProductCode { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? CoverImage { get; set; }

    public string? CategoryName { get; set; }

    public string 分類 { get; set; } = null!;
}
