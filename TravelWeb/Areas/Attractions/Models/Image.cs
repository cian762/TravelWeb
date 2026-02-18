using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Areas.Attractions.Models;

public partial class Image
{
    [Key]
    public int ImageId { get; set; } // 這是主鍵

    public int? AttractionId { get; set; } // 這是外鍵

    public string? ImagePath { get; set; }

    public virtual Attraction? Attraction { get; set; }


}
