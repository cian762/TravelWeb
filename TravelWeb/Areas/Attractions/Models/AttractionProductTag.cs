using System;
using System.Collections.Generic;
using TravelWeb.Areas.Attractions.Models;

namespace TravelWeb.Models
{
    public partial class AttractionProductTag
    {
        public int ProductId { get; set; } // 對應 product_id
        public int TagId { get; set; }     // 對應 tag_id

        public virtual AttractionProduct Product { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
