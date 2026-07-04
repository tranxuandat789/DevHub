using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class ServicePackage
{
    public int ServiceId { get; set; }

    public string PackageName { get; set; } = null!;

    public string Title { get; set; } = null!;

    public decimal Price { get; set; }

    public int Credit { get; set; }

    public int MaxPosts { get; set; }

    public int? DurationDays { get; set; }

    public int? PriorityPush { get; set; }

    public bool? HasAiChatbot { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PackageTransaction> PackageTransactions { get; set; } = new List<PackageTransaction>();

    public virtual ICollection<CompanyPackageHistory> CompanyPackageHistories { get; set; } = new List<CompanyPackageHistory>();
}
