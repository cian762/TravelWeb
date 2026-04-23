using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 👈 務必加入這個 namespace

namespace TravelWeb.Areas.Attractions.Models;

[Table("AttractionTypeCategories", Schema = "Attractions")] // 👈 指定正確的 Table 與 Schema
public partial class AttractionTypeCategory
{
    [Key] // 👈 標記主鍵
    [Column("attraction_type_id")] // 👈 對接 SQL 的底線名稱
    public int AttractionTypeId { get; set; }

    [Column("attraction_type_name")] // 👈 對接 SQL 的底線名稱
    public string AttractionTypeName { get; set; } = null!;
}