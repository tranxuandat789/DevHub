-- =====================================================
-- DevHub Database Schema  SQL Server 
-- =====================================================
-- CREATE DATABASE DevHub
USE DevHub;
-- =====================================================
-- Create User Account Table (Base Table)
-- =====================================================
CREATE TABLE [user_account] (
    [user_id] INT PRIMARY KEY IDENTITY(1, 1),
    [email] NVARCHAR(255) NOT NULL UNIQUE,
    [password_hash] NVARCHAR(255) NULL,
    [google_id] NVARCHAR(255),
    [user_type] NVARCHAR(20) NOT NULL CHECK (
        [user_type] IN ('ADMIN', 'RECRUITER', 'MODERATOR', 'CANDIDATE')
    ),
    [is_active] BIT DEFAULT 1,
    [created_at] DATETIME DEFAULT GETDATE(),
    [last_login] DATETIME NULL,
    [last_updated] DATETIME DEFAULT GETDATE(),
    [reset_password_token] NVARCHAR(255) NULL,
    [reset_password_expires_at] DATETIME NULL,
    [otp_verification] NVARCHAR(255) NULL,
    [otp_expires_at] DATETIME NULL
);
-- GOOGLE ID IS NOT REQUIRED, BUT HAS TO BE UNIQUE
CREATE UNIQUE NONCLUSTERED INDEX UQ_user_google_id ON user_account(google_id)
WHERE google_id IS NOT NULL;
-- =====================================================
-- Create Admin Table
-- =====================================================
CREATE TABLE [admin] (
    [admin_id] INT PRIMARY KEY NOT NULL,
    [username] NVARCHAR(100) NOT NULL UNIQUE,
    [full_name] NVARCHAR(255) NULL,
    [role] NVARCHAR(50) NULL CHECK ([role] IN ('ADMIN', 'MODERATOR')),
    CONSTRAINT [FK__admin__admin_id] FOREIGN KEY ([admin_id]) REFERENCES [user_account]([user_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Candidate Table
-- =====================================================
CREATE TABLE [candidate] (
    [candidate_id] INT PRIMARY KEY NOT NULL,
    [full_name] NVARCHAR(255) NOT NULL,
    [gender] NVARCHAR(20) NULL,
    [birthdate] DATE NULL,
    [phone] NVARCHAR(20) NULL,
    [address] NVARCHAR(255) NULL,
    [expected_salary_min] DECIMAL(18, 2) NULL,
    [expected_salary_max] DECIMAL(18, 2) NULL,
    [preferred_location] NVARCHAR(100) NULL,
    [preferred_working_model] NVARCHAR(50) NULL,
    [experience_years] INT NULL,
    [cv_searchable] BIT DEFAULT 1,
    [profile_completion] INT DEFAULT 0,
    [image_url] NVARCHAR(500) NULL,
    [social_media_url] NVARCHAR(500) NULL,
    CONSTRAINT [FK__candidate__candi] FOREIGN KEY ([candidate_id]) REFERENCES [user_account]([user_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Company Table
-- =====================================================
CREATE TABLE [company] (
    [company_id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_name] NVARCHAR(255) NOT NULL,
    [company_address] NVARCHAR(255),
    [company_logo_url] NVARCHAR(500),
    [company_description] NVARCHAR(MAX),
    [website] NVARCHAR(255),
    [industry] NVARCHAR(100),
    [tax_code] NVARCHAR(50),
    [business_license_url] NVARCHAR(500),
    [additional_documents_url] NVARCHAR(500),
    [total_spent] DECIMAL(18, 2) DEFAULT 0,
    [average_rating] DECIMAL(3, 2) DEFAULT 0.00,
    [total_reviews] INT DEFAULT 0,
    [is_verified] BIT DEFAULT 0,
    [profile_completion] INT DEFAULT 0,
    [status] NVARCHAR(20) DEFAULT 'PENDING' CHECK ([status] IN ('PENDING', 'APPROVED', 'REJECTED'))
);
-- =====================================================
-- Create Company Invitation Table
-- =====================================================
CREATE TABLE [company_invitation] (
    [invitation_id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_id] INT NOT NULL,
    [email] NVARCHAR(255) NOT NULL,
    [token] NVARCHAR(255) NOT NULL,
    [expires_at] DATETIME NOT NULL,
    [status] NVARCHAR(20) DEFAULT 'PENDING' CHECK ([status] IN ('PENDING', 'ACCEPTED', 'EXPIRED')),
    [created_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__company_invitation__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE CASCADE
);
-- =====================================================
-- Create Recruiter Table
-- =====================================================
CREATE TABLE [recruiter] (
    [recruiter_id] INT PRIMARY KEY NOT NULL,
    [full_name] NVARCHAR(255) NOT NULL,
    [position] NVARCHAR(100),
    [phone] NVARCHAR(20),
    [company_id] INT NULL,
    [is_company_admin] BIT DEFAULT 0,
    CONSTRAINT [FK__recruiter__recru] FOREIGN KEY ([recruiter_id]) REFERENCES [user_account]([user_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK__recruiter__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Common Job Position Table
-- =====================================================
CREATE TABLE [common_job_position] (
    [position_id] INT PRIMARY KEY IDENTITY(1, 1),
    [position_name] NVARCHAR(150) NOT NULL UNIQUE,
    [is_active] BIT DEFAULT 1,
    [created_at] DATETIME DEFAULT GETDATE()
);
-- =====================================================
-- Create Common Technology Table
-- =====================================================
CREATE TABLE [common_technology] (
    [tech_id] INT PRIMARY KEY IDENTITY(1, 1),
    [tech_name] NVARCHAR(100) NOT NULL UNIQUE,
    [category] NVARCHAR(50) NULL,
    [is_active] BIT DEFAULT 1,
    [created_at] DATETIME DEFAULT GETDATE()
);
-- =====================================================
-- Create CV Table
-- =====================================================
CREATE TABLE [cv] (
    [cv_id] INT PRIMARY KEY IDENTITY(1, 1),
    [candidate_id] INT NOT NULL,
    [title] NVARCHAR(255) NULL,
    [cv_url] NVARCHAR(500) NULL,
    [skills] NVARCHAR(MAX) NULL,
    [experience] NVARCHAR(MAX) NULL,
    [education] NVARCHAR(MAX) NULL,
    [languages] NVARCHAR(MAX) NULL,
    [is_default] BIT DEFAULT 0,
    [created_at] DATETIME DEFAULT GETDATE(),
    [updated_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__cv__candidate_id] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE CASCADE
);
-- =====================================================
-- Create Candidate Skill Table
-- =====================================================
CREATE TABLE [candidate_skill] (
    [candidate_id] INT NOT NULL,
    [tech_id] INT NOT NULL,
    [level] NVARCHAR(30) NULL,
    CONSTRAINT [PK__candidate_skill] PRIMARY KEY ([candidate_id], [tech_id]),
    CONSTRAINT [FK__candidate_skill__candidate] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__candidate_skill__tech] FOREIGN KEY ([tech_id]) REFERENCES [common_technology]([tech_id]) ON DELETE CASCADE
);
-- =====================================================
-- Create Service Package Table
-- =====================================================
CREATE TABLE [service_package] (
    [service_id] INT PRIMARY KEY IDENTITY(1, 1),
    [package_name] NVARCHAR(100) NULL,
    [title] NVARCHAR(255) NULL,
    [description] NVARCHAR(MAX) NULL,
    [price] DECIMAL(18, 2) NULL,
    [credit] INT NULL,
    [max_posts] INT NULL,
    [duration_days] INT NULL,
    [priority_push] INT DEFAULT 0,
    [has_ai_chatbot] BIT DEFAULT 0,
    [image_url] NVARCHAR(500) NULL,
    [is_active] BIT DEFAULT 1,
    [created_at] DATETIME DEFAULT GETDATE()
);
-- =====================================================
-- Create Promotion Table
-- =====================================================
CREATE TABLE [promotion] (
    [promotion_id] INT PRIMARY KEY IDENTITY(1, 1),
    [promo_code] NVARCHAR(50) NOT NULL UNIQUE,
    [title] NVARCHAR(255) NULL,
    [discount_percent] DECIMAL(5, 2) NULL,
    [max_discount] DECIMAL(18, 2) NULL,
    [quantity] INT NULL,
    [start_date] DATE NULL,
    [end_date] DATE NULL,
    [is_active] BIT DEFAULT 1,
    [created_at] DATETIME DEFAULT GETDATE()
);
-- =====================================================
-- Create Package Transaction Table (VNPay Payments)
-- =====================================================
CREATE TABLE [package_transaction] (
    [transaction_id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_id] INT NOT NULL,
    [service_id] INT NULL,
    [amount_vnd] DECIMAL(18, 2) NOT NULL,
    [discount_amount] DECIMAL(18, 2) DEFAULT 0,
    [final_amount] DECIMAL(18, 2) NOT NULL,
    [payment_method] NVARCHAR(50) DEFAULT 'vnpay',
    [vnpay_txn_ref] NVARCHAR(100) NULL,
    [vnpay_transaction_no] NVARCHAR(100) NULL,
    [vnpay_bank_code] NVARCHAR(20) NULL,
    [status] NVARCHAR(30) DEFAULT 'PENDING' CHECK (
        [status] IN (
            'PENDING',
            'SUCCESS',
            'FAILED',
            'CANCELLED',
            'REFUNDED'
        )
    ),
    [transaction_type] NVARCHAR(50) NOT NULL,
    [promotion_id] INT NULL,
    [description] NVARCHAR(500),
    [transaction_date] DATETIME DEFAULT GETDATE(),
    [completed_at] DATETIME NULL,
    CONSTRAINT [FK__package_transaction__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__package_transaction__service] FOREIGN KEY ([service_id]) REFERENCES [service_package]([service_id]),
    CONSTRAINT [FK__package_transaction__promotion] FOREIGN KEY ([promotion_id]) REFERENCES [promotion]([promotion_id])
);
-- Create Package Transaction Indexes
CREATE NONCLUSTERED INDEX [idx_payment_company] ON [package_transaction]([company_id]);
CREATE NONCLUSTERED INDEX [idx_payment_status] ON [package_transaction]([status]);
CREATE NONCLUSTERED INDEX [idx_payment_vnpay_ref] ON [package_transaction]([vnpay_txn_ref]);
-- =====================================================
-- Create Company Package History Table
-- =====================================================
CREATE TABLE [company_package_history] (
    [id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_id] INT NOT NULL,
    [service_id] INT NOT NULL,
    [transaction_id] INT NOT NULL,
    [posts_granted] INT NOT NULL,
    [posts_remaining] INT NOT NULL,
    [promotions_remaining] INT NOT NULL DEFAULT 0,
    [is_active] BIT DEFAULT 1,
    [start_date] DATETIME DEFAULT GETDATE(),
    [end_date] DATETIME NULL,
    [price_at_purchase] DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CONSTRAINT [FK__company_history__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__company_history__service] FOREIGN KEY ([service_id]) REFERENCES [service_package]([service_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__company_history__transaction] FOREIGN KEY ([transaction_id]) REFERENCES [package_transaction]([transaction_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Province Table
-- =====================================================
CREATE TABLE [province] (
    [province_id] INT PRIMARY KEY IDENTITY(1, 1),
    [province_name] NVARCHAR(100) NOT NULL UNIQUE
);
-- =====================================================
-- Create Job Post Table
-- =====================================================
CREATE TABLE [job_post] (
    [job_id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_id] INT NOT NULL,
    [position_id] INT NOT NULL,
    [moderator_id] INT NULL,
    [company_package_history_id] INT,
    [title] NVARCHAR(255) NOT NULL,
    [description] NVARCHAR(MAX) NULL,
    [requirement] NVARCHAR(MAX) NULL,
    [benefit] NVARCHAR(MAX) NULL,
    [skill] NVARCHAR(255) NULL,
    [experience_level] NVARCHAR(50) NULL,
    [working_model] NVARCHAR(50) NULL,
    [salary_type] NVARCHAR(20) NOT NULL DEFAULT 'RANGE' CHECK (
        [salary_type] IN ('RANGE', 'FROM', 'UPTO', 'NEGOTIABLE')
    ),
    [salary_min] DECIMAL(18, 2) NULL,
    [salary_max] DECIMAL(18, 2) NULL,
    [hiring_quota] INT NULL,
    [deadline] DATE NULL,
    [status] NVARCHAR(20) DEFAULT 'PENDING' CHECK (
        [status] IN (
            'PENDING',
            'APPROVED',
            'REJECTED',
            'FINISHED',
            'CLOSED',
            'EXPIRED'
        )
    ),
    [priority_score] INT DEFAULT 0,
    [application_count] INT DEFAULT 0,
    [approved_at] DATETIME NULL,
    [rejected_reason] NVARCHAR(500) NULL,
    [created_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__job_post__position] FOREIGN KEY ([position_id]) REFERENCES [common_job_position]([position_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__job_post__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]),
    CONSTRAINT [FK__job_post__moderator] FOREIGN KEY ([moderator_id]) REFERENCES [admin]([admin_id]),
    CONSTRAINT [FK__job_post__package_history] FOREIGN KEY ([company_package_history_id]) REFERENCES [company_package_history]([id])
);
-- Create indexes for job_post
CREATE NONCLUSTERED INDEX [idx_job_post_company] ON [job_post]([company_id]);
CREATE NONCLUSTERED INDEX [idx_job_post_status] ON [job_post]([status]);
-- =====================================================
-- Create Job Post Province Table
-- =====================================================
CREATE TABLE [job_post_province] (
    [job_id] INT NOT NULL,
    [province_id] INT NOT NULL,
    CONSTRAINT [PK_job_post_province] PRIMARY KEY ([job_id], [province_id]),
    CONSTRAINT [FK_job_post_province_job] FOREIGN KEY ([job_id]) REFERENCES [job_post]([job_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_job_post_province_province] FOREIGN KEY ([province_id]) REFERENCES [province]([province_id]) ON DELETE CASCADE
);
CREATE INDEX [idx_job_post_province_province] ON [job_post_province] ([province_id]);
-- =====================================================
-- Create Job Tech Stack Table (Many-to-Many)
-- =====================================================
CREATE TABLE [job_tech_stack] (
    [job_id] INT NOT NULL,
    [tech_id] INT NOT NULL,
    CONSTRAINT [PK__job_tech_stack] PRIMARY KEY ([job_id], [tech_id]),
    CONSTRAINT [FK__job_tech_stack__job] FOREIGN KEY ([job_id]) REFERENCES [job_post]([job_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__job_tech_stack__tech] FOREIGN KEY ([tech_id]) REFERENCES [common_technology]([tech_id]) ON DELETE CASCADE
);
-- Create index for job_tech_stack lookup
CREATE NONCLUSTERED INDEX [idx_job_tech_stack_lookup] ON [job_tech_stack]([tech_id], [job_id]);
-- =====================================================
-- Create Application Table
-- =====================================================
CREATE TABLE [application] (
    [application_id] INT PRIMARY KEY IDENTITY(1, 1),
    [job_id] INT NOT NULL,
    [candidate_id] INT NOT NULL,
    [cv_id] INT NOT NULL,
    [cover_letter] NVARCHAR(MAX) NULL,
    [notes] NVARCHAR(MAX) NULL,
    [status] NVARCHAR(30) DEFAULT 'PENDING' CHECK (
        [status] IN (
            'PENDING',
            'APPROVED',
            'REJECTED',
            'FINISHED',
            'HIRED',
            'CANCELLED',
            'FAILED'
        )
    ),
    [applied_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__application__job] FOREIGN KEY ([job_id]) REFERENCES [job_post]([job_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__application__candidate] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK__application__cv] FOREIGN KEY ([cv_id]) REFERENCES [cv]([cv_id]) ON DELETE NO ACTION
);
-- Create indexes for application
CREATE NONCLUSTERED INDEX [idx_application_job] ON [application]([job_id]);
CREATE NONCLUSTERED INDEX [idx_application_candidate] ON [application]([candidate_id]);
-- =====================================================
-- Create Interview Table
-- =====================================================
CREATE TABLE [interview] (
    [interview_id] INT PRIMARY KEY IDENTITY(1, 1),
    [application_id] INT NOT NULL,
    [candidate_id] INT NOT NULL,
    [recruiter_id] INT NOT NULL,
    [location] NVARCHAR(255) NULL,
    [meeting_link] NVARCHAR(500) NULL,
    [scheduled_time] DATETIME NULL,
    [notes] NVARCHAR(MAX) NULL,
    [status] NVARCHAR(20) DEFAULT 'SCHEDULED' CHECK (
        [status] IN (
            'SCHEDULED',
            'PENDING',
            'FINISHED',
            'EXPIRED',
            'CANCELLED'
        )
    ),
    [created_at] DATETIME DEFAULT GETDATE(),
    [updated_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__interview__application] FOREIGN KEY ([application_id]) REFERENCES [application]([application_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__interview__candidate] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK__interview__recruiter] FOREIGN KEY ([recruiter_id]) REFERENCES [recruiter]([recruiter_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Bookmark Table
-- =====================================================
CREATE TABLE [bookmark] (
    [bookmark_id] INT PRIMARY KEY IDENTITY(1, 1),
    [candidate_id] INT NOT NULL,
    [job_id] INT NOT NULL,
    [created_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__bookmark__candidate] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__bookmark__job] FOREIGN KEY ([job_id]) REFERENCES [job_post]([job_id]) ON DELETE CASCADE
);
-- =====================================================
-- Create Review Company Table
-- =====================================================
CREATE TABLE [review_company] (
    [review_id] INT PRIMARY KEY IDENTITY(1, 1),
    [candidate_id] INT NOT NULL,
    [company_id] INT NOT NULL,
    [moderator_id] INT NULL,
    [rating] INT NULL,
    [pros] NVARCHAR(MAX) NULL,
    [cons] NVARCHAR(MAX) NULL,
    [description] NVARCHAR(MAX) NULL,
    [is_anonymous] BIT DEFAULT 0,
    [status] NVARCHAR(20) DEFAULT 'PENDING' CHECK ([status] IN ('PENDING', 'APPROVED', 'REJECTED')),
    [rejection_reason] NVARCHAR(500) NULL,
    [created_at] DATETIME DEFAULT GETDATE(),
    [updated_at] DATETIME DEFAULT GETDATE(),
    [moderated_at] DATETIME NULL,
    CONSTRAINT [UQ_candidate_company_review] UNIQUE ([candidate_id], [company_id]),
    CONSTRAINT [FK__review_company__candidate] FOREIGN KEY ([candidate_id]) REFERENCES [candidate]([candidate_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__review_company__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__review_company__moderator] FOREIGN KEY ([moderator_id]) REFERENCES [admin]([admin_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Blog Post Table
-- =====================================================
CREATE TABLE [blog_post] (
    [blog_id] INT PRIMARY KEY IDENTITY(1, 1),
    [publisher_id] INT NULL,
    [title] NVARCHAR(255) NULL,
    [slug] NVARCHAR(255) NOT NULL UNIQUE,
	[tag] NVARCHAR(255) NOT NULL UNIQUE,
    [content] NVARCHAR(MAX) NULL,
    [thumbnail_url] NVARCHAR(500) NULL,
    [is_published] BIT DEFAULT 0,
    [created_at] DATETIME DEFAULT GETDATE(),
    [published_at] DATETIME NULL,
    [status] INT DEFAULT 3,
    CONSTRAINT [FK__blog_post__publisher] FOREIGN KEY ([publisher_id]) REFERENCES [admin]([admin_id]) ON DELETE CASCADE
);
-- Create index for blog_post slug
CREATE NONCLUSTERED INDEX [idx_blog_post_slug] ON [blog_post]([slug]);
-- =====================================================
-- Create Article Table (For Recruiters/Companies)
-- =====================================================
CREATE TABLE [article] (
    [article_id] INT PRIMARY KEY IDENTITY(1, 1),
    [company_id] INT NOT NULL,
    [title] NVARCHAR(255) NULL,
    [slug] NVARCHAR(255) NOT NULL UNIQUE,
    [content] NVARCHAR(MAX) NULL,
    [thumbnail_url] NVARCHAR(500) NULL,
    [status] NVARCHAR(20) DEFAULT 'PENDING' CHECK ([status] IN ('PENDING', 'APPROVED', 'REJECTED')),
    [created_at] DATETIME DEFAULT GETDATE(),
    [updated_at] DATETIME DEFAULT GETDATE(),
    [approved_at] DATETIME NULL,
    [approver_id] INT NULL,
    [reject_reason] NVARCHAR(500) NULL,
    CONSTRAINT [FK__article__company] FOREIGN KEY ([company_id]) REFERENCES [company]([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__article__approver] FOREIGN KEY ([approver_id]) REFERENCES [admin]([admin_id]) ON DELETE NO ACTION
);
-- =====================================================
-- Create Mod Tier Assignment Table
-- =====================================================
CREATE TABLE [mod_tier_assignment] (
    [assignment_id] INT PRIMARY KEY IDENTITY(1, 1),
    [moderator_id] INT NOT NULL,
    [service_id] INT NOT NULL,
    [created_at] DATETIME DEFAULT GETDATE(),
    CONSTRAINT [FK__mod_tier_assignment__mod] FOREIGN KEY ([moderator_id]) REFERENCES [admin]([admin_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__mod_tier_assignment__service] FOREIGN KEY ([service_id]) REFERENCES [service_package]([service_id]) ON DELETE CASCADE,
    CONSTRAINT [UQ__mod_tier_assignment] UNIQUE ([moderator_id], [service_id])
);
-- =====================================================
-- Data normalization: uppercase enum-like fields
-- =====================================================
UPDATE [user_account]
SET [user_type] = UPPER([user_type])
WHERE [user_type] IS NOT NULL;
UPDATE [admin]
SET [role] = UPPER([role])
WHERE [role] IS NOT NULL;
UPDATE [package_transaction]
SET [status] = UPPER([status])
WHERE [status] IS NOT NULL;
UPDATE [job_post]
SET [status] = UPPER([status])
WHERE [status] IS NOT NULL;
UPDATE [application]
SET [status] = UPPER([status])
WHERE [status] IS NOT NULL;
UPDATE [interview]
SET [status] = UPPER([status])
WHERE [status] IS NOT NULL;
UPDATE [review_company]
SET [status] = UPPER([status])
WHERE [status] IS NOT NULL;
-- =====================================================
-- Create Notification Table
-- =====================================================
CREATE TABLE [notification] (
    [notification_id] INT PRIMARY KEY IDENTITY(1, 1),
    [user_id] INT NOT NULL,
    [user_type] NVARCHAR(20) NULL,
    [type] NVARCHAR(50) NULL,
    [title] NVARCHAR(255) NULL,
    [message] NVARCHAR(500) NULL,
    [reference_type] NVARCHAR(50) NULL,
    [reference_id] INT NULL,
    [severity_level] NVARCHAR(20) DEFAULT 'Low',
    [is_read] BIT DEFAULT 0,
    [created_at] DATETIME DEFAULT GETDATE()
);
-- Create index for notification user
CREATE NONCLUSTERED INDEX [idx_notification_user] ON [notification]([user_id], [user_type]);
-- =====================================================
-- Create Audit Log Table
-- =====================================================
CREATE TABLE [audit_log] (
    [log_id] INT PRIMARY KEY IDENTITY(1, 1),
    [user_id] INT NULL,
    [user_type] NVARCHAR(20) NULL,
    [entity_type] NVARCHAR(50) NULL,
    [entity_id] INT NULL,
    [action] NVARCHAR(100) NULL,
    [old_value] NVARCHAR(MAX) NULL,
    [new_value] NVARCHAR(MAX) NULL,
    [ip_address] NVARCHAR(50) NULL,
    [created_at] DATETIME DEFAULT GETDATE()
);
