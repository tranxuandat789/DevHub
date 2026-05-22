using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class BlogPost
{
    public int BlogId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Content { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? Author { get; set; }

    public int PublisherId { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Admin Publisher { get; set; } = null!;
}
