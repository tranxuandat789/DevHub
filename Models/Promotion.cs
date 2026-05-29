using System;
using System.Collections.Generic;

namespace DevHub.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string Title { get; set; } = null!;

    public string PromoCode { get; set; } = null!;

    public decimal? DiscountPercent { get; set; }

    public decimal? MaxDiscount { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PackageTransaction> PackageTransactions { get; set; } = new List<PackageTransaction>();
}
