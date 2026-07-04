using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyAddress { get; set; }

    public string? CompanyLogoUrl { get; set; }

    public string? CompanyDescription { get; set; }

    public string? Website { get; set; }

    public string? Industry { get; set; }

    public string? TaxCode { get; set; }

    public string? BusinessLicenseUrl { get; set; }

    public string? AdditionalDocumentsUrl { get; set; }

    public decimal? TotalSpent { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalReviews { get; set; }

    public bool? IsVerified { get; set; }

    public int? ProfileCompletion { get; set; }

    public string Status { get; set; } = "PENDING";

    public virtual ICollection<Recruiter> Recruiters { get; set; } = new List<Recruiter>();

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    public virtual ICollection<CompanyPackageHistory> CompanyPackageHistories { get; set; } = new List<CompanyPackageHistory>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<PackageTransaction> PackageTransactions { get; set; } = new List<PackageTransaction>();

    public virtual ICollection<ReviewCompany> ReviewCompanies { get; set; } = new List<ReviewCompany>();

    public virtual ICollection<CompanyInvitation> CompanyInvitations { get; set; } = new List<CompanyInvitation>();
}
