using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class CommentLike
{
    public int CommentId { get; set; }

    public int UserId { get; set; }

    public virtual Comment Comment { get; set; } = null!;
}
