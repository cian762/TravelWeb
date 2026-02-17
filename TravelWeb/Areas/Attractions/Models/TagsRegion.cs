using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models
{
    public partial class TagsRegion
    {
      
        public int RegionId { get; set; } // PK
        public int? Uid { get; set; }     // 父層ID，允許 Null
        public string RegionName { get; set; } = null!; // 不允許 Null

        // 導覽屬性 (非必填，但加了方便之後做階層查詢)
        // 加上這一行，讓 Attraction 認識它
        public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    }
}
