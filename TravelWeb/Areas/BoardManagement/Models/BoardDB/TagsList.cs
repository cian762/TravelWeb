using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class TagsList
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;
}
