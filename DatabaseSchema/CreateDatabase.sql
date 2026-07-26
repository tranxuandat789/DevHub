IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [audit_log] (
    [log_id] int NOT NULL IDENTITY,
    [user_id] int NULL,
    [user_type] nvarchar(20) NULL,
    [action] nvarchar(100) NOT NULL,
    [entity_type] nvarchar(50) NULL,
    [entity_id] int NULL,
    [old_value] nvarchar(max) NULL,
    [new_value] nvarchar(max) NULL,
    [ip_address] nvarchar(50) NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__audit_lo__9E2397E0562E6B47] PRIMARY KEY ([log_id])
);
GO

CREATE TABLE [common_job_position] (
    [position_id] int NOT NULL IDENTITY,
    [position_name] nvarchar(150) NOT NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__common_j__99A0E7A487B920C3] PRIMARY KEY ([position_id])
);
GO

CREATE TABLE [common_technology] (
    [tech_id] int NOT NULL IDENTITY,
    [tech_name] nvarchar(100) NOT NULL,
    [category] nvarchar(50) NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__common_t__E0D7817A3C28CA0C] PRIMARY KEY ([tech_id])
);
GO

CREATE TABLE [company] (
    [company_id] int NOT NULL IDENTITY,
    [company_name] nvarchar(255) NOT NULL,
    [company_address] nvarchar(255) NULL,
    [company_logo_url] nvarchar(500) NULL,
    [company_description] nvarchar(max) NULL,
    [website] nvarchar(255) NULL,
    [industry] nvarchar(100) NULL,
    [tax_code] nvarchar(50) NULL,
    [business_license_url] nvarchar(500) NULL,
    [additional_documents_url] nvarchar(500) NULL,
    [total_spent] decimal(18,2) NULL DEFAULT 0.0,
    [average_rating] decimal(3,2) NULL DEFAULT 0.0,
    [total_reviews] int NULL DEFAULT 0,
    [is_verified] bit NULL DEFAULT CAST(0 AS bit),
    [profile_completion] int NULL DEFAULT 0,
    [status] nvarchar(20) NOT NULL DEFAULT N'PENDING',
    CONSTRAINT [PK_company] PRIMARY KEY ([company_id])
);
GO

CREATE TABLE [notification] (
    [notification_id] int NOT NULL IDENTITY,
    [user_id] int NOT NULL,
    [user_type] nvarchar(20) NOT NULL,
    [title] nvarchar(255) NOT NULL,
    [message] nvarchar(500) NOT NULL,
    [type] nvarchar(50) NULL,
    [is_read] bit NULL DEFAULT CAST(0 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    [severity_level] nvarchar(20) NULL DEFAULT N'Low',
    [reference_id] int NULL,
    [reference_type] nvarchar(50) NULL,
    CONSTRAINT [PK__notifica__E059842F06EAA605] PRIMARY KEY ([notification_id])
);
GO

CREATE TABLE [promotion] (
    [promotion_id] int NOT NULL IDENTITY,
    [title] nvarchar(255) NOT NULL,
    [promo_code] nvarchar(50) NOT NULL,
    [discount_percent] decimal(5,2) NULL,
    [max_discount] decimal(18,2) NULL,
    [start_date] date NULL,
    [end_date] date NULL,
    [quantity] int NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__promotio__2CB9556B27C78245] PRIMARY KEY ([promotion_id])
);
GO

CREATE TABLE [province] (
    [province_id] int NOT NULL IDENTITY,
    [province_name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_province] PRIMARY KEY ([province_id])
);
GO

CREATE TABLE [service_package] (
    [service_id] int NOT NULL IDENTITY,
    [package_name] nvarchar(100) NOT NULL,
    [title] nvarchar(255) NOT NULL,
    [price] decimal(18,2) NOT NULL,
    [credit] int NOT NULL,
    [max_posts] int NOT NULL,
    [duration_days] int NULL,
    [priority_push] int NULL DEFAULT 0,
    [has_ai_chatbot] bit NULL DEFAULT CAST(0 AS bit),
    [description] nvarchar(max) NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [image_url] nvarchar(500) NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__service___3E0DB8AF78E35848] PRIMARY KEY ([service_id])
);
GO

CREATE TABLE [user_account] (
    [user_id] int NOT NULL IDENTITY,
    [google_id] nvarchar(255) NULL,
    [email] nvarchar(255) NOT NULL,
    [password_hash] nvarchar(255) NULL,
    [otp_verification] nvarchar(255) NULL,
    [otp_expires_at] datetime NULL,
    [reset_password_token] nvarchar(100) NULL,
    [reset_password_expires_at] datetime NULL,
    [user_type] nvarchar(20) NOT NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    [last_login] datetime NULL,
    [last_updated] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__user_acc__B9BE370FB27D0E4F] PRIMARY KEY ([user_id])
);
GO

CREATE TABLE [company_invitation] (
    [invitation_id] int NOT NULL IDENTITY,
    [company_id] int NOT NULL,
    [email] nvarchar(255) NOT NULL,
    [token] nvarchar(255) NOT NULL,
    [expires_at] datetime NOT NULL,
    [status] nvarchar(20) NOT NULL DEFAULT N'PENDING',
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_company_invitation] PRIMARY KEY ([invitation_id]),
    CONSTRAINT [FK__company_invitation__company] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]) ON DELETE CASCADE
);
GO

CREATE TABLE [package_transaction] (
    [transaction_id] int NOT NULL IDENTITY,
    [company_id] int NOT NULL,
    [service_id] int NULL,
    [amount_vnd] decimal(18,2) NOT NULL,
    [discount_amount] decimal(18,2) NOT NULL DEFAULT 0.0,
    [final_amount] decimal(18,2) NOT NULL,
    [payment_method] nvarchar(50) NOT NULL DEFAULT N'vnpay',
    [vnpay_txn_ref] nvarchar(100) NULL,
    [vnpay_transaction_no] nvarchar(100) NULL,
    [vnpay_bank_code] nvarchar(20) NULL,
    [status] nvarchar(30) NOT NULL DEFAULT N'pending',
    [transaction_type] nvarchar(50) NOT NULL,
    [promotion_id] int NULL,
    [description] nvarchar(500) NULL,
    [transaction_date] datetime NULL DEFAULT ((getdate())),
    [completed_at] datetime NULL,
    CONSTRAINT [PK__package___85C600AFBBD9338E] PRIMARY KEY ([transaction_id]),
    CONSTRAINT [FK__package_transaction__company] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__package_transaction__promotion] FOREIGN KEY ([promotion_id]) REFERENCES [promotion] ([promotion_id]),
    CONSTRAINT [FK__package_transaction__service] FOREIGN KEY ([service_id]) REFERENCES [service_package] ([service_id])
);
GO

CREATE TABLE [admin] (
    [admin_id] int NOT NULL,
    [username] nvarchar(100) NOT NULL,
    [full_name] nvarchar(255) NULL,
    [role] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__admin__43AA41418C23432B] PRIMARY KEY ([admin_id]),
    CONSTRAINT [FK__admin__admin_id__571DF1D5] FOREIGN KEY ([admin_id]) REFERENCES [user_account] ([user_id])
);
GO

CREATE TABLE [candidate] (
    [candidate_id] int NOT NULL,
    [full_name] nvarchar(255) NOT NULL,
    [gender] nvarchar(20) NULL,
    [birthdate] date NULL,
    [phone] nvarchar(20) NULL,
    [address] nvarchar(255) NULL,
    [expected_salary_min] decimal(18,2) NULL,
    [expected_salary_max] decimal(18,2) NULL,
    [preferred_location] nvarchar(100) NULL,
    [preferred_working_model] nvarchar(50) NULL,
    [experience_years] int NULL,
    [cv_searchable] bit NULL DEFAULT CAST(1 AS bit),
    [profile_completion] int NULL DEFAULT 0,
    [image_url] nvarchar(500) NULL,
    [social_media_url] nvarchar(500) NULL,
    CONSTRAINT [PK__candidat__39BD4C187D159B41] PRIMARY KEY ([candidate_id]),
    CONSTRAINT [FK__candidate__candi__49C3F6B7] FOREIGN KEY ([candidate_id]) REFERENCES [user_account] ([user_id])
);
GO

CREATE TABLE [recruiter] (
    [recruiter_id] int NOT NULL,
    [full_name] nvarchar(255) NOT NULL,
    [position] nvarchar(100) NULL,
    [phone] nvarchar(20) NULL,
    [company_id] int NULL,
    [is_company_admin] bit NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK__recruite__42ABA2576945C9D7] PRIMARY KEY ([recruiter_id]),
    CONSTRAINT [FK__recruiter__company] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]),
    CONSTRAINT [FK__recruiter__recru__534D60F1] FOREIGN KEY ([recruiter_id]) REFERENCES [user_account] ([user_id])
);
GO

CREATE TABLE [company_package_history] (
    [id] int NOT NULL IDENTITY,
    [company_id] int NOT NULL,
    [service_id] int NOT NULL,
    [transaction_id] int NOT NULL,
    [posts_granted] int NOT NULL,
    [posts_remaining] int NOT NULL,
    [promotions_remaining] int NOT NULL DEFAULT 0,
    [start_date] datetime NULL DEFAULT ((getdate())),
    [end_date] datetime NULL,
    [is_active] bit NULL DEFAULT CAST(1 AS bit),
    [price_at_purchase] decimal(18,2) NOT NULL DEFAULT 0.0,
    CONSTRAINT [PK__recruite__3213E83FA34D72CF] PRIMARY KEY ([id]),
    CONSTRAINT [FK__recruiter_history__recruiter] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__recruiter_history__service] FOREIGN KEY ([service_id]) REFERENCES [service_package] ([service_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__recruiter_history__transaction] FOREIGN KEY ([transaction_id]) REFERENCES [package_transaction] ([transaction_id])
);
GO

CREATE TABLE [article] (
    [article_id] int NOT NULL IDENTITY,
    [company_id] int NOT NULL,
    [title] nvarchar(255) NULL,
    [slug] nvarchar(255) NOT NULL,
    [content] nvarchar(max) NULL,
    [thumbnail_url] nvarchar(500) NULL,
    [status] nvarchar(20) NULL DEFAULT N'PENDING',
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    [approved_at] datetime NULL,
    [approver_id] int NULL,
    [reject_reason] nvarchar(500) NULL,
    [AdminId] int NULL,
    CONSTRAINT [PK_article] PRIMARY KEY ([article_id]),
    CONSTRAINT [FK__article__approver] FOREIGN KEY ([approver_id]) REFERENCES [admin] ([admin_id]),
    CONSTRAINT [FK__article__company] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_article_admin_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [admin] ([admin_id])
);
GO

CREATE TABLE [blog_post] (
    [blog_id] int NOT NULL IDENTITY,
    [publisher_id] int NULL,
    [title] nvarchar(255) NULL,
    [slug] nvarchar(255) NOT NULL,
    [tag] nvarchar(255) NOT NULL,
    [content] nvarchar(max) NULL,
    [thumbnail_url] nvarchar(500) NULL,
    [is_published] bit NULL DEFAULT CAST(0 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    [published_at] datetime NULL,
    [status] int NULL DEFAULT 3,
    CONSTRAINT [PK__blog_pos__2975AA28BA813DA3] PRIMARY KEY ([blog_id]),
    CONSTRAINT [FK__blog_post__publisher] FOREIGN KEY ([publisher_id]) REFERENCES [admin] ([admin_id]) ON DELETE CASCADE
);
GO

CREATE TABLE [mod_tier_assignment] (
    [assignment_id] int NOT NULL IDENTITY,
    [moderator_id] int NOT NULL,
    [service_id] int NOT NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [AdminId] int NULL,
    CONSTRAINT [PK_mod_tier_assignment] PRIMARY KEY ([assignment_id]),
    CONSTRAINT [FK__mod_tier__moderator] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__mod_tier__service] FOREIGN KEY ([service_id]) REFERENCES [service_package] ([service_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_mod_tier_assignment_admin_AdminId] FOREIGN KEY ([AdminId]) REFERENCES [admin] ([admin_id])
);
GO

CREATE TABLE [candidate_skill] (
    [candidate_id] int NOT NULL,
    [tech_id] int NOT NULL,
    [level] nvarchar(30) NULL,
    CONSTRAINT [PK__candidat__87B0340FA344E306] PRIMARY KEY ([candidate_id], [tech_id]),
    CONSTRAINT [FK__candidate__candi__68487DD7] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id]),
    CONSTRAINT [FK__candidate__tech___693CA210] FOREIGN KEY ([tech_id]) REFERENCES [common_technology] ([tech_id])
);
GO

CREATE TABLE [cv] (
    [cv_id] int NOT NULL IDENTITY,
    [candidate_id] int NOT NULL,
    [title] nvarchar(255) NOT NULL,
    [education] nvarchar(max) NULL,
    [experience] nvarchar(max) NULL,
    [skills] nvarchar(max) NULL,
    [languages] nvarchar(max) NULL,
    [cv_url] nvarchar(500) NULL,
    [is_default] bit NULL DEFAULT CAST(0 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__cv__C36883E6D4E21DAA] PRIMARY KEY ([cv_id]),
    CONSTRAINT [FK__cv__candidate_id__6EF57B66] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id])
);
GO

CREATE TABLE [review_company] (
    [review_id] int NOT NULL IDENTITY,
    [candidate_id] int NOT NULL,
    [company_id] int NOT NULL,
    [rating] int NOT NULL,
    [pros] nvarchar(max) NOT NULL,
    [cons] nvarchar(max) NOT NULL,
    [description] nvarchar(max) NULL,
    [is_anonymous] bit NULL DEFAULT CAST(0 AS bit),
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    [status] nvarchar(20) NOT NULL DEFAULT N'pending',
    [rejection_reason] nvarchar(500) NULL,
    [moderator_id] int NULL,
    [moderated_at] datetime2 NULL,
    CONSTRAINT [PK__review_r__60883D90CFCBFACD] PRIMARY KEY ([review_id]),
    CONSTRAINT [FK__review_re__candi__2DE6D218] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id]),
    CONSTRAINT [FK__review_re__recru__2EDAF651] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]),
    CONSTRAINT [FK_review_company_admin_moderator_id] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id])
);
GO

CREATE TABLE [job_post] (
    [job_id] int NOT NULL IDENTITY,
    [company_id] int NOT NULL,
    [title] nvarchar(255) NOT NULL,
    [position_id] int NOT NULL,
    [skill] nvarchar(255) NULL,
    [working_model] nvarchar(50) NOT NULL,
    [salary_type] nvarchar(20) NOT NULL DEFAULT N'RANGE',
    [salary_min] decimal(18,2) NULL,
    [salary_max] decimal(18,2) NULL,
    [experience_level] nvarchar(50) NULL,
    [description] nvarchar(max) NULL,
    [requirement] nvarchar(max) NULL,
    [benefit] nvarchar(max) NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [deadline] date NULL,
    [status] nvarchar(20) NULL DEFAULT N'PENDING',
    [priority_score] int NULL DEFAULT 0,
    [hiring_quota] int NULL,
    [application_count] int NULL DEFAULT 0,
    [approved_at] datetime NULL,
    [moderator_id] int NULL,
    [company_package_history_id] int NULL,
    [rejected_reason] nvarchar(500) NULL,
    CONSTRAINT [PK__job_post__6E32B6A56D928E5F] PRIMARY KEY ([job_id]),
    CONSTRAINT [FK__job_post__company] FOREIGN KEY ([company_id]) REFERENCES [company] ([company_id]),
    CONSTRAINT [FK__job_post__modera__619B8048] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id]),
    CONSTRAINT [FK__job_post__package_history] FOREIGN KEY ([company_package_history_id]) REFERENCES [company_package_history] ([id]),
    CONSTRAINT [FK__job_post__positi__60A75C0F] FOREIGN KEY ([position_id]) REFERENCES [common_job_position] ([position_id])
);
GO

CREATE TABLE [application] (
    [application_id] int NOT NULL IDENTITY,
    [candidate_id] int NOT NULL,
    [cv_id] int NOT NULL,
    [job_id] int NOT NULL,
    [cover_letter] nvarchar(max) NULL,
    [applied_at] datetime NULL DEFAULT ((getdate())),
    [status] nvarchar(30) NULL DEFAULT N'pending',
    [notes] nvarchar(max) NULL,
    CONSTRAINT [PK__applicat__3BCBDCF26F68278F] PRIMARY KEY ([application_id]),
    CONSTRAINT [FK__applicati__candi__73BA3083] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id]),
    CONSTRAINT [FK__applicati__cv_id__74AE54BC] FOREIGN KEY ([cv_id]) REFERENCES [cv] ([cv_id]),
    CONSTRAINT [FK__applicati__job_i__75A278F5] FOREIGN KEY ([job_id]) REFERENCES [job_post] ([job_id])
);
GO

CREATE TABLE [bookmark] (
    [bookmark_id] int NOT NULL IDENTITY,
    [candidate_id] int NOT NULL,
    [job_id] int NOT NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__bookmark__D9C65802FC133ABD] PRIMARY KEY ([bookmark_id]),
    CONSTRAINT [FK__bookmark__candid__32AB8735] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id]),
    CONSTRAINT [FK__bookmark__job_id__339FAB6E] FOREIGN KEY ([job_id]) REFERENCES [job_post] ([job_id])
);
GO

CREATE TABLE [job_post_province] (
    [job_id] int NOT NULL,
    [province_id] int NOT NULL,
    CONSTRAINT [PK_job_post_province] PRIMARY KEY ([job_id], [province_id]),
    CONSTRAINT [FK_job_post_province_job] FOREIGN KEY ([job_id]) REFERENCES [job_post] ([job_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_job_post_province_province] FOREIGN KEY ([province_id]) REFERENCES [province] ([province_id]) ON DELETE CASCADE
);
GO

CREATE TABLE [job_tech_stack] (
    [job_id] int NOT NULL,
    [tech_id] int NOT NULL,
    CONSTRAINT [PK__job_tech__D03FCEB2D9718A37] PRIMARY KEY ([job_id], [tech_id]),
    CONSTRAINT [FK__job_tech___job_i__6477ECF3] FOREIGN KEY ([job_id]) REFERENCES [job_post] ([job_id]) ON DELETE CASCADE,
    CONSTRAINT [FK__job_tech___tech___656C112C] FOREIGN KEY ([tech_id]) REFERENCES [common_technology] ([tech_id])
);
GO

CREATE TABLE [interview] (
    [interview_id] int NOT NULL IDENTITY,
    [application_id] int NOT NULL,
    [recruiter_id] int NOT NULL,
    [candidate_id] int NOT NULL,
    [scheduled_time] datetime NOT NULL,
    [meeting_link] nvarchar(500) NULL,
    [location] nvarchar(255) NULL,
    [status] nvarchar(20) NULL DEFAULT N'scheduled',
    [notes] nvarchar(max) NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__intervie__141E55525829E99E] PRIMARY KEY ([interview_id]),
    CONSTRAINT [FK__interview__appli__7C4F7684] FOREIGN KEY ([application_id]) REFERENCES [application] ([application_id]),
    CONSTRAINT [FK__interview__candi__7E37BEF6] FOREIGN KEY ([candidate_id]) REFERENCES [candidate] ([candidate_id]),
    CONSTRAINT [FK__interview__recru__7D439ABD] FOREIGN KEY ([recruiter_id]) REFERENCES [recruiter] ([recruiter_id])
);
GO

CREATE UNIQUE INDEX [UQ__admin__F3DBC5727CFAE85F] ON [admin] ([username]);
GO

CREATE INDEX [idx_application_candidate] ON [application] ([candidate_id]);
GO

CREATE INDEX [idx_application_job] ON [application] ([job_id]);
GO

CREATE INDEX [IX_application_cv_id] ON [application] ([cv_id]);
GO

CREATE INDEX [IX_article_AdminId] ON [article] ([AdminId]);
GO

CREATE INDEX [IX_article_approver_id] ON [article] ([approver_id]);
GO

CREATE INDEX [IX_article_company_id] ON [article] ([company_id]);
GO

CREATE UNIQUE INDEX [IX_article_slug] ON [article] ([slug]);
GO

CREATE INDEX [IX_blog_post_publisher_id] ON [blog_post] ([publisher_id]);
GO

CREATE UNIQUE INDEX [IX_blog_post_tag] ON [blog_post] ([tag]);
GO

CREATE UNIQUE INDEX [UQ__blog_pos__32DD1E4C47481A2E] ON [blog_post] ([slug]);
GO

CREATE INDEX [IX_bookmark_candidate_id] ON [bookmark] ([candidate_id]);
GO

CREATE INDEX [IX_bookmark_job_id] ON [bookmark] ([job_id]);
GO

CREATE INDEX [IX_candidate_skill_tech_id] ON [candidate_skill] ([tech_id]);
GO

CREATE UNIQUE INDEX [UQ__common_j__0030EAD7A7C738AF] ON [common_job_position] ([position_name]);
GO

CREATE UNIQUE INDEX [UQ__common_t__AC1EA1F13AC03AE9] ON [common_technology] ([tech_name]);
GO

CREATE INDEX [IX_company_invitation_company_id] ON [company_invitation] ([company_id]);
GO

CREATE INDEX [IX_company_package_history_company_id] ON [company_package_history] ([company_id]);
GO

CREATE INDEX [IX_company_package_history_service_id] ON [company_package_history] ([service_id]);
GO

CREATE INDEX [IX_company_package_history_transaction_id] ON [company_package_history] ([transaction_id]);
GO

CREATE INDEX [IX_cv_candidate_id] ON [cv] ([candidate_id]);
GO

CREATE INDEX [IX_interview_application_id] ON [interview] ([application_id]);
GO

CREATE INDEX [IX_interview_candidate_id] ON [interview] ([candidate_id]);
GO

CREATE INDEX [IX_interview_recruiter_id] ON [interview] ([recruiter_id]);
GO

CREATE INDEX [idx_job_post_company] ON [job_post] ([company_id]);
GO

CREATE INDEX [idx_job_post_status] ON [job_post] ([status]);
GO

CREATE INDEX [IX_job_post_company_package_history_id] ON [job_post] ([company_package_history_id]);
GO

CREATE INDEX [IX_job_post_moderator_id] ON [job_post] ([moderator_id]);
GO

CREATE INDEX [IX_job_post_position_id] ON [job_post] ([position_id]);
GO

CREATE INDEX [idx_job_post_province_province] ON [job_post_province] ([province_id]);
GO

CREATE INDEX [idx_job_tech_stack_lookup] ON [job_tech_stack] ([tech_id], [job_id]);
GO

CREATE INDEX [IX_mod_tier_assignment_AdminId] ON [mod_tier_assignment] ([AdminId]);
GO

CREATE INDEX [IX_mod_tier_assignment_moderator_id] ON [mod_tier_assignment] ([moderator_id]);
GO

CREATE INDEX [IX_mod_tier_assignment_service_id] ON [mod_tier_assignment] ([service_id]);
GO

CREATE INDEX [idx_notification_user] ON [notification] ([user_id], [user_type]);
GO

CREATE INDEX [IX_package_transaction_company_id] ON [package_transaction] ([company_id]);
GO

CREATE INDEX [IX_package_transaction_promotion_id] ON [package_transaction] ([promotion_id]);
GO

CREATE INDEX [IX_package_transaction_service_id] ON [package_transaction] ([service_id]);
GO

CREATE UNIQUE INDEX [UQ__promotio__C07E23151D5A3120] ON [promotion] ([promo_code]);
GO

CREATE UNIQUE INDEX [UQ_province_name] ON [province] ([province_name]);
GO

CREATE INDEX [IX_recruiter_company_id] ON [recruiter] ([company_id]);
GO

CREATE INDEX [IX_review_company_company_id] ON [review_company] ([company_id]);
GO

CREATE INDEX [IX_review_company_moderator_id] ON [review_company] ([moderator_id]);
GO

CREATE UNIQUE INDEX [UQ_candidate_company_review] ON [review_company] ([candidate_id], [company_id]);
GO

CREATE UNIQUE INDEX [UQ__user_acc__AB6E6164EFF0C5ED] ON [user_account] ([email]);
GO

CREATE UNIQUE INDEX [UQ_user_google_id] ON [user_account] ([google_id]) WHERE ([google_id] IS NOT NULL);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260704231941_InitialCreate', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [interview] ADD [interview_type] nvarchar(50) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260707103726_UpdateDatabaseSchema', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [package_transaction] ADD [buyer_tax_code] nvarchar(50) NULL;
GO

ALTER TABLE [package_transaction] ADD [total_amount] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [package_transaction] ADD [vat_amount] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [package_transaction] ADD [vat_rate] decimal(5,2) NOT NULL DEFAULT 8.0;
GO

ALTER TABLE [company] ADD [moderator_id] int NULL;
GO

CREATE TABLE [moderator_task_type] (
    [id] int NOT NULL IDENTITY,
    [moderator_id] int NOT NULL,
    [task_type] nvarchar(30) NOT NULL,
    [assigned_by] int NOT NULL,
    [created_at] datetime NULL DEFAULT ((getdate())),
    [updated_at] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_moderator_task_type] PRIMARY KEY ([id]),
    CONSTRAINT [FK__mod_task_type__assigned_by] FOREIGN KEY ([assigned_by]) REFERENCES [admin] ([admin_id]),
    CONSTRAINT [FK__mod_task_type__moderator] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_company_moderator_id] ON [company] ([moderator_id]);
GO

CREATE INDEX [IX_moderator_task_type_assigned_by] ON [moderator_task_type] ([assigned_by]);
GO

CREATE UNIQUE INDEX [IX_moderator_task_type_moderator_id] ON [moderator_task_type] ([moderator_id]);
GO

ALTER TABLE [company] ADD CONSTRAINT [FK__company__moderator_id] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id]) ON DELETE SET NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260709033438_AddVATColumns', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [review_company] ADD [salary_rating] int NULL;
GO

ALTER TABLE [review_company] ADD [training_rating] int NULL;
GO

ALTER TABLE [review_company] ADD [care_rating] int NULL;
GO

ALTER TABLE [review_company] ADD [culture_rating] int NULL;
GO

ALTER TABLE [review_company] ADD [workspace_rating] int NULL;
GO

ALTER TABLE [review_company] ADD [ot_policy] nvarchar(20) NULL;
GO

ALTER TABLE [review_company] ADD [recommend] bit NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260709153400_AddReviewCompanyRatings', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

                DECLARE @ConstraintName nvarchar(200);
                SELECT @ConstraintName = name
                FROM sys.key_constraints
                WHERE type = 'UQ' AND parent_object_id = OBJECT_ID('blog_post');
                IF @ConstraintName IS NOT NULL
                BEGIN
                    EXEC('ALTER TABLE blog_post DROP CONSTRAINT ' + @ConstraintName);
                END
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260710030020_DropBlogPostTagUniqueConstraint', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [interview] DROP CONSTRAINT [FK__interview__recru__7D439ABD];
GO

DROP INDEX [IX_interview_recruiter_id] ON [interview];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[interview]') AND [c].[name] = N'recruiter_id');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [interview] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [interview] DROP COLUMN [recruiter_id];
GO

CREATE TABLE [moderator_industry_assignment] (
    [id] int NOT NULL IDENTITY,
    [moderator_id] int NOT NULL,
    [task_type] nvarchar(30) NOT NULL,
    [industry] nvarchar(100) NOT NULL,
    [assigned_by] int NOT NULL,
    [created_at] datetime NOT NULL DEFAULT ((getdate())),
    [updated_at] datetime NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_moderator_industry_assignment] PRIMARY KEY ([id]),
    CONSTRAINT [FK_mod_industry_assigned_by] FOREIGN KEY ([assigned_by]) REFERENCES [admin] ([admin_id]),
    CONSTRAINT [FK_mod_industry_moderator] FOREIGN KEY ([moderator_id]) REFERENCES [admin] ([admin_id])
);
GO

CREATE UNIQUE INDEX [IX_mod_industry_task_industry] ON [moderator_industry_assignment] ([task_type], [industry]);
GO

CREATE INDEX [IX_moderator_industry_assignment_assigned_by] ON [moderator_industry_assignment] ([assigned_by]);
GO

CREATE INDEX [IX_moderator_industry_assignment_moderator_id] ON [moderator_industry_assignment] ([moderator_id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714120149_RemoveRecruiterIdFromInterview', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260717105738_SyncSchemaChanges', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [blog_post] ADD [author_name] nvarchar(255) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260717110459_AddAuthorNameToBlogPost', N'8.0.1');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[user_account].[EmailNotificationsEnabled]', N'email_notifications_enabled', N'COLUMN';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260717111159_MapEmailNotificationsEnabled', N'8.0.1');
GO

COMMIT;
GO

