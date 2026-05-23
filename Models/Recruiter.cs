using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Recruiter
{
    public int RecruiterId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Phone { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyAddress { get; set; }

    public string? CompanyLogoUrl { get; set; }

    public string? CompanyDescription { get; set; }

    public string? Website { get; set; }

    public string? Industry { get; set; }

    public string? TaxCode { get; set; }

    public string? BusinessLicenseUrl { get; set; }

    public string? AdditionalDocumentsUrl { get; set; }

    public int? PostCredit { get; set; }

    public int? TotalCoinTopup { get; set; }

    public DateTime? LastTopupDate { get; set; }

    public decimal? TotalSpent { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalReviews { get; set; }

    public bool? IsVerified { get; set; }

    public int? ProfileCompletion { get; set; }

    public virtual ICollection<CoinTransaction> CoinTransactions { get; set; } = new List<CoinTransaction>();

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<PackageTransaction> PackageTransactions { get; set; } = new List<PackageTransaction>();

    public virtual UserAccount RecruiterNavigation { get; set; } = null!;

    public virtual ICollection<RecruiterPackageHistory> RecruiterPackageHistories { get; set; } = new List<RecruiterPackageHistory>();

    public virtual ICollection<ReviewRecruiter> ReviewRecruiters { get; set; } = new List<ReviewRecruiter>();
}
