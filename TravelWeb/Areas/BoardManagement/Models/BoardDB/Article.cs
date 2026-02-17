using System;
using System.Collections.Generic;

namespace TravelWeb.Areas.BoardManagement.Models.BoardDB;

public partial class Article
{
    public int ArticleId { get; set; }

    public string UserId { get; set; } = null!;

    public string? Title { get; set; }

    public byte Type { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte Status { get; set; }

    public bool IsViolation { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
