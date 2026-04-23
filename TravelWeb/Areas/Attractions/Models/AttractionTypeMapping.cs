using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // 👈 務必加入

namespace TravelWeb.Areas.Attractions.Models;

[Table("AttractionTypeMappings", Schema = "Attractions")] // 👈 指定帶 s 的名稱
public partial class AttractionTypeMapping
{
    [Column("attraction_type_id")] // 👈 對接底線名稱
    public int AttractionTypeId { get; set; }

    [Column("attraction_id")] // 👈 對接底線名稱
    public int AttractionId { get; set; }

    public virtual Attraction? Attraction { get; set; }

    public virtual AttractionTypeCategory? AttractionType { get; set; }
}