using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class CommentPhoto
{
    public int CommentId { get; set; }

    public string? Photo { get; set; }

    public virtual Comment Comment { get; set; } = null!;
}
