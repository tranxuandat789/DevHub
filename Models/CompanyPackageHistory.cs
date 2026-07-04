using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class CompanyPackageHistory
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int ServiceId { get; set; }

    public int TransactionId { get; set; }

    public int PostsGranted { get; set; }

    public int PostsRemaining { get; set; }

    public int PromotionsRemaining { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ServicePackage Service { get; set; } = null!;

    public virtual PackageTransaction Transaction { get; set; } = null!;

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}
