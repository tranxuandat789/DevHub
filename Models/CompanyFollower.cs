using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevHub.Models
{
    [Table("company_follower")]
    public class CompanyFollower
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("candidate_id")]
        public int CandidateId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("created_at", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; } = null!;

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;
    }
}
