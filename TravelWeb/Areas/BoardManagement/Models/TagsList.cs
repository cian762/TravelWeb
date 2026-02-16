using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models;

public partial class TagsList
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;
}
