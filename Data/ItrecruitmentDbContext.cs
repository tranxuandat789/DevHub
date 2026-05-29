using System;
using System.Collections.Generic;
using DevHub.Models;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Data;

public partial class ItrecruitmentDbContext : DbContext
{
    public ItrecruitmentDbContext()
    {
    }

    public ItrecruitmentDbContext(DbContextOptions<ItrecruitmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Bookmark> Bookmarks { get; set; }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<CandidateSkill> CandidateSkills { get; set; }

    public virtual DbSet<CommonJobPosition> CommonJobPositions { get; set; }

    public virtual DbSet<CommonTechnology> CommonTechnologies { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<JobPost> JobPosts { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PackageTransaction> PackageTransactions { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Recruiter> Recruiters { get; set; }

    public virtual DbSet<RecruiterPackageHistory> RecruiterPackageHistories { get; set; }

    public virtual DbSet<ReviewRecruiter> ReviewRecruiters { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__admin__43AA41418C23432B");

            entity.ToTable("admin");

            entity.HasIndex(e => e.Username, "UQ__admin__F3DBC5727CFAE85F").IsUnique();

            entity.Property(e => e.AdminId)
                .ValueGeneratedNever()
                .HasColumnName("admin_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.AdminNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__admin__admin_id__571DF1D5");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__applicat__3BCBDCF26F68278F");

            entity.ToTable("application");

            entity.HasIndex(e => e.CandidateId, "idx_application_candidate");

            entity.HasIndex(e => e.JobId, "idx_application_job");

            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("applied_at");
            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.CoverLetter).HasColumnName("cover_letter");
            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__applicati__candi__73BA3083");

            entity.HasOne(d => d.Cv).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CvId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__applicati__cv_id__74AE54BC");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__applicati__job_i__75A278F5");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__audit_lo__9E2397E0562E6B47");

            entity.ToTable("audit_log");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .HasColumnName("user_type");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__blog_pos__2975AA28BA813DA3");

            entity.ToTable("blog_post");

            entity.HasIndex(e => e.Slug, "UQ__blog_pos__32DD1E4C47481A2E").IsUnique();

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(false)
                .HasColumnName("is_published");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("published_at");
            entity.Property(e => e.PublisherId).HasColumnName("publisher_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(500)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Publisher).WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__blog_post__publi__395884C4");
            entity.HasOne(d => d.AuthorRecruiter)
                .WithMany(p => p.BlogPosts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.BookmarkId).HasName("PK__bookmark__D9C65802FC133ABD");

            entity.ToTable("bookmark");

            entity.Property(e => e.BookmarkId).HasColumnName("bookmark_id");
            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.JobId).HasColumnName("job_id");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookmark__candid__32AB8735");

            entity.HasOne(d => d.Job).WithMany(p => p.Bookmarks)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookmark__job_id__339FAB6E");
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.CandidateId).HasName("PK__candidat__39BD4C187D159B41");

            entity.ToTable("candidate");

            entity.Property(e => e.CandidateId)
                .ValueGeneratedNever()
                .HasColumnName("candidate_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.CvSearchable)
                .HasDefaultValue(true)
                .HasColumnName("cv_searchable");
            entity.Property(e => e.ExpectedSalaryMax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("expected_salary_max");
            entity.Property(e => e.ExpectedSalaryMin)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("expected_salary_min");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .HasColumnName("gender");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PreferredLocation)
                .HasMaxLength(100)
                .HasColumnName("preferred_location");
            entity.Property(e => e.ProfileCompletion)
                .HasDefaultValue(0)
                .HasColumnName("profile_completion");
            entity.Property(e => e.SocialMediaUrl)
                .HasMaxLength(500)
                .HasColumnName("social_media_url");

            entity.HasOne(d => d.CandidateNavigation).WithOne(p => p.Candidate)
                .HasForeignKey<Candidate>(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__candidate__candi__49C3F6B7");
        });

        modelBuilder.Entity<CandidateSkill>(entity =>
        {
            entity.HasKey(e => new { e.CandidateId, e.TechId }).HasName("PK__candidat__87B0340FA344E306");

            entity.ToTable("candidate_skill");

            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.TechId).HasColumnName("tech_id");
            entity.Property(e => e.Level)
                .HasMaxLength(30)
                .HasColumnName("level");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__candidate__candi__68487DD7");

            entity.HasOne(d => d.Tech).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.TechId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__candidate__tech___693CA210");
        });

        modelBuilder.Entity<CommonJobPosition>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__common_j__99A0E7A487B920C3");

            entity.ToTable("common_job_position");

            entity.HasIndex(e => e.PositionName, "UQ__common_j__0030EAD7A7C738AF").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PositionName)
                .HasMaxLength(150)
                .HasColumnName("position_name");
        });

        modelBuilder.Entity<CommonTechnology>(entity =>
        {
            entity.HasKey(e => e.TechId).HasName("PK__common_t__E0D7817A3C28CA0C");

            entity.ToTable("common_technology");

            entity.HasIndex(e => e.TechName, "UQ__common_t__AC1EA1F13AC03AE9").IsUnique();

            entity.Property(e => e.TechId).HasColumnName("tech_id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TechName)
                .HasMaxLength(100)
                .HasColumnName("tech_name");
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.CvId).HasName("PK__cv__C36883E6D4E21DAA");

            entity.ToTable("cv");

            entity.Property(e => e.CvId).HasColumnName("cv_id");
            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CvUrl)
                .HasMaxLength(500)
                .HasColumnName("cv_url");
            entity.Property(e => e.Education).HasColumnName("education");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");
            entity.Property(e => e.Languages).HasColumnName("languages");
            entity.Property(e => e.Skills).HasColumnName("skills");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cv__candidate_id__6EF57B66");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__intervie__141E55525829E99E");

            entity.ToTable("interview");

            entity.Property(e => e.InterviewId).HasColumnName("interview_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.MeetingLink)
                .HasMaxLength(500)
                .HasColumnName("meeting_link");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.RecruiterId).HasColumnName("recruiter_id");
            entity.Property(e => e.ScheduledTime)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_time");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("scheduled")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__interview__appli__7C4F7684");

            entity.HasOne(d => d.Candidate).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__interview__candi__7E37BEF6");

            entity.HasOne(d => d.Recruiter).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__interview__recru__7D439ABD");
        });

        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__job_post__6E32B6A56D928E5F");

            entity.ToTable("job_post");

            entity.HasIndex(e => e.RecruiterId, "idx_job_post_recruiter");

            entity.HasIndex(e => e.Status, "idx_job_post_status");

            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.ApplicationCount)
                .HasDefaultValue(0)
                .HasColumnName("application_count");
            entity.Property(e => e.ApprovedAt)
                .HasColumnType("datetime")
                .HasColumnName("approved_at");
            entity.Property(e => e.Benefit).HasColumnName("benefit");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ExperienceLevel)
                .HasMaxLength(50)
                .HasColumnName("experience_level");
            entity.Property(e => e.IsPromoted)
                .HasDefaultValue(false)
                .HasColumnName("is_promoted");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.ModeratorId).HasColumnName("moderator_id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.RecruiterPackageHistoryId).HasColumnName("recruiter_package_history_id");
            entity.Property(e => e.PriorityScore)
                .HasDefaultValue(0)
                .HasColumnName("priority_score");
            entity.Property(e => e.RecruiterId).HasColumnName("recruiter_id");
            entity.Property(e => e.RejectedReason)
                .HasMaxLength(500)
                .HasColumnName("rejected_reason");
            entity.Property(e => e.Requirement).HasColumnName("requirement");
            entity.Property(e => e.SalaryMax)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("salary_max");
            entity.Property(e => e.SalaryMin)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("salary_min");
            entity.Property(e => e.Skill)
                .HasMaxLength(255)
                .HasColumnName("skill");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.HiringQuota)
                    .HasColumnName("hiring_quota");
            entity.Property(e => e.WorkingModel)
                .HasMaxLength(50)
                .HasColumnName("working_model");

            entity.HasOne(d => d.Moderator).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.ModeratorId)
                .HasConstraintName("FK__job_post__modera__619B8048");

            entity.HasOne(d => d.Position).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__job_post__positi__60A75C0F");

            entity.HasOne(d => d.Recruiter).WithMany()
                .HasForeignKey(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__job_post__recruiter");

            entity.HasOne(d => d.RecruiterPackageHistory).WithMany(p => p.JobPosts)
                .HasForeignKey(d => d.RecruiterPackageHistoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__job_post__package_history");

            entity.HasMany(d => d.Teches).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobTechStack",
                    r => r.HasOne<CommonTechnology>().WithMany()
                        .HasForeignKey("TechId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__job_tech___tech___656C112C"),
                    l => l.HasOne<JobPost>().WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK__job_tech___job_i__6477ECF3"),
                    j =>
                    {
                        j.HasKey("JobId", "TechId").HasName("PK__job_tech__D03FCEB2D9718A37");
                        j.ToTable("job_tech_stack");
                        j.HasIndex(new[] { "TechId", "JobId" }, "idx_job_tech_stack_lookup");
                        j.IndexerProperty<int>("JobId").HasColumnName("job_id");
                        j.IndexerProperty<int>("TechId").HasColumnName("tech_id");
                    });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F06EAA605");

            entity.ToTable("notification");

            entity.HasIndex(e => new { e.UserId, e.UserType }, "idx_notification_user");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .HasColumnName("message");
            entity.Property(e => e.ReferenceId).HasColumnName("reference_id");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(50)
                .HasColumnName("reference_type");
            entity.Property(e => e.SeverityLevel)
                .HasMaxLength(20)
                .HasDefaultValue("Low")
                .HasColumnName("severity_level");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .HasColumnName("user_type");
        });

        modelBuilder.Entity<PackageTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__package___85C600AFBBD9338E");

            entity.ToTable("package_transaction");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.AmountVnd)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount_vnd");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discount_amount")
                .HasDefaultValue(0m);
            entity.Property(e => e.FinalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("final_amount");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method")
                .HasDefaultValue("vnpay");
            entity.Property(e => e.VnpayTxnRef)
                .HasMaxLength(100)
                .HasColumnName("vnpay_txn_ref");
            entity.Property(e => e.VnpayTransactionNo)
                .HasMaxLength(100)
                .HasColumnName("vnpay_transaction_no");
            entity.Property(e => e.VnpayBankCode)
                .HasMaxLength(20)
                .HasColumnName("vnpay_bank_code");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status")
                .HasDefaultValue("pending");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .HasColumnName("transaction_type");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.RecruiterId).HasColumnName("recruiter_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("transaction_date");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("completed_at");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PackageTransactions)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK__package_transaction__promotion");

            entity.HasOne(d => d.Recruiter).WithMany(p => p.PackageTransactions)
                .HasForeignKey(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__package_transaction__recruiter");

            entity.HasOne(d => d.Service).WithMany(p => p.PackageTransactions)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__package_transaction__service");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__promotio__2CB9556B27C78245");

            entity.ToTable("promotion");

            entity.HasIndex(e => e.PromoCode, "UQ__promotio__C07E23151D5A3120").IsUnique();

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxDiscount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("max_discount");
            entity.Property(e => e.PromoCode)
                .HasMaxLength(50)
                .HasColumnName("promo_code");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.HasKey(e => e.RecruiterId).HasName("PK__recruite__42ABA2576945C9D7");

            entity.ToTable("recruiter");

            entity.Property(e => e.RecruiterId)
                .ValueGeneratedNever()
                .HasColumnName("recruiter_id");
            entity.Property(e => e.AdditionalDocumentsUrl)
                .HasMaxLength(500)
                .HasColumnName("additional_documents_url");
            entity.Property(e => e.AverageRating)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("average_rating");
            entity.Property(e => e.BusinessLicenseUrl)
                .HasMaxLength(500)
                .HasColumnName("business_license_url");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(255)
                .HasColumnName("company_address");
            entity.Property(e => e.CompanyDescription).HasColumnName("company_description");
            entity.Property(e => e.CompanyLogoUrl)
                .HasMaxLength(500)
                .HasColumnName("company_logo_url");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Industry)
                .HasMaxLength(100)
                .HasColumnName("industry");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.ProfileCompletion)
                .HasDefaultValue(0)
                .HasColumnName("profile_completion");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .HasColumnName("tax_code");
            entity.Property(e => e.TotalReviews)
                .HasDefaultValue(0)
                .HasColumnName("total_reviews");
            entity.Property(e => e.TotalSpent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_spent");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");

            entity.HasOne(d => d.RecruiterNavigation).WithOne(p => p.Recruiter)
                .HasForeignKey<Recruiter>(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__recruiter__recru__534D60F1");
        });

        modelBuilder.Entity<RecruiterPackageHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__recruite__3213E83FA34D72CF");

            entity.ToTable("recruiter_package_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.PostsGranted).HasColumnName("posts_granted");
            entity.Property(e => e.PostsRemaining).HasColumnName("posts_remaining");
            entity.Property(e => e.PromotionsRemaining)
                .HasDefaultValue(0)
                .HasColumnName("promotions_remaining");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PriceAtPurchase)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_at_purchase")
                .HasDefaultValue(0m);
            entity.Property(e => e.RecruiterId).HasColumnName("recruiter_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("start_date");

            entity.HasOne(d => d.Recruiter).WithMany(p => p.RecruiterPackageHistories)
                .HasForeignKey(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__recruiter_history__recruiter");

            entity.HasOne(d => d.Service).WithMany(p => p.RecruiterPackageHistories)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__recruiter_history__service");

            entity.HasOne(d => d.Transaction).WithMany(p => p.RecruiterPackageHistories)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__recruiter_history__transaction");
        });

        modelBuilder.Entity<ReviewRecruiter>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__review_r__60883D90CFCBFACD");

            entity.ToTable("review_recruiter");

            entity.HasIndex(e => new { e.CandidateId, e.RecruiterId }, "UQ_candidate_recruiter_review").IsUnique();

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.CandidateId).HasColumnName("candidate_id");
            entity.Property(e => e.Cons).HasColumnName("cons");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsAnonymous)
                .HasDefaultValue(false)
                .HasColumnName("is_anonymous");
            entity.Property(e => e.Pros).HasColumnName("pros");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RecruiterId).HasColumnName("recruiter_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Status)
                  .HasColumnName("status")
                  .HasMaxLength(20)
                  .HasDefaultValue("pending");
            entity.Property(e => e.RejectionReason)
                  .HasColumnName("rejection_reason")
                  .HasMaxLength(500);
            entity.Property(e => e.ModeratorId)
                  .HasColumnName("moderator_id");
            entity.Property(e => e.ModeratedAt)
                  .HasColumnName("moderated_at");
            entity.HasOne(d => d.Moderator)
                  .WithMany()
                  .HasForeignKey(d => d.ModeratorId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.Candidate).WithMany(p => p.ReviewRecruiters)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review_re__candi__2DE6D218");

            entity.HasOne(d => d.Recruiter).WithMany(p => p.ReviewRecruiters)
                .HasForeignKey(d => d.RecruiterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review_re__recru__2EDAF651");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__service___3E0DB8AF78E35848");

            entity.ToTable("service_package");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Credit).HasColumnName("credit");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.HasAiChatbot)
                .HasDefaultValue(false)
                .HasColumnName("has_ai_chatbot");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxPosts).HasColumnName("max_posts");
            entity.Property(e => e.PackageName)
                .HasMaxLength(100)
                .HasColumnName("package_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PriorityPush)
                .HasDefaultValue(0)
                .HasColumnName("priority_push");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_acc__B9BE370FB27D0E4F");

            entity.ToTable("user_account");

            entity.HasIndex(e => e.Email, "UQ__user_acc__AB6E6164EFF0C5ED").IsUnique();

            entity.HasIndex(e => e.GoogleId, "UQ_user_google_id")
                .IsUnique()
                .HasFilter("([google_id] IS NOT NULL)");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .HasColumnName("google_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .HasColumnName("user_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
