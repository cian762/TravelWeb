using System;
using System.Collections.Generic;

namespace TravelWeb.Models.Travel;

public partial class Article
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public byte Type { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte Status { get; set; }

    public bool IsViolation { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
