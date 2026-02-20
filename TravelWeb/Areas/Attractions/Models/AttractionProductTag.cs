using System;
using System.Collections.Generic;
using TravelWeb.Areas.Attractions.Models;

namespace TravelWeb.Models
{
    public partial class AttractionProductTag
    {
        // 1. 定義兩個主鍵欄位
        public int ProductId { get; set; }
        public int TagId { get; set; }

        // 2. 導覽屬性 (Navigation Properties)
        public virtual AttractionProduct Product { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
