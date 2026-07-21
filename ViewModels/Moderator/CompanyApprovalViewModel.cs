namespace DevHub.ViewModels.Moderator
{
    public class CompanyApprovalViewModel
    {
        public int LogId { get; set; }
        public int RecruiterId { get; set; }
        public required string CompanyName { get; set; }
        public required string TaxCode { get; set; }
        public required string BusinessLicenseUrl { get; set; }
        public required string AdditionalDocumentsUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? RequestedAt { get; set; }
    }
}
