using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string? FullName { get; set; }

    public string Role { get; set; } = null!;

    public virtual UserAccount AdminNavigation { get; set; } = null!;

    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}
