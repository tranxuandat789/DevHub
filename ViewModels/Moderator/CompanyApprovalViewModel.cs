using System;

namespace DevHub.ViewModels.Moderator
{
    public class CompanyVerificationRequestViewModel
    {
        public int LogId { get; set; }
        public int RecruiterId { get; set; }
        public required string CompanyName { get; set; }
        public required string TaxCode { get; set; }
        public required string BusinessLicenseUrl { get; set; }
        public required string AdditionalDocumentsUrl { get; set; }
        public DateTime? RequestedAt { get; set; }
    }
}
