using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.Attractions.Models;

public partial class TicketType
{
    // ID 是主鍵，絕對不能是 Null
    public int TicketTypeCode { get; set; }

    // 名稱在資料庫是不允許 Null 的
    public string TicketTypeName { get; set; } = null!;

    // 資料庫勾選了允許 Null，所以這裡一定要加問號 (?)
    public int? SortOrder { get; set; }

    public virtual ICollection<AttractionProduct> AttractionProducts { get; set; } = new List<AttractionProduct>();
}
