using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class TagsList
{
    
    public int TagId { get; set; }

    public string? icon { get; set; }

    public string TagName { get; set; } = null!;
}
