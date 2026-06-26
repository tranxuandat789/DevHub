USE ITRecruitmentDB;
GO

SET NOCOUNT ON;

/* ============================================================
   Reconcile blog_post with the model (teammate's blog feature).

   Vì sao cần script này:
   - DB này tạo từ SQL script (database-first); bảng blog_post đang ở cấu trúc CŨ
     (is_published, author_id, FK FK__blog_post__publisher / FK__blog_post__author).
   - Migration 20260625100316_AddTagsToBlogPost GIẢ ĐỊNH blog_post đã có sẵn
     status / is_deleted / approver_id / reject_reason (nó ALTER chứ không ADD)
     => chạy `dotnet ef database update` sẽ lỗi "FK__blog_post__approver is not a constraint".
   - Script này thêm các cột/FK còn thiếu cho khớp Models/BlogPost.cs + DbContext,
     rồi BASELINE migration để EF coi như đã áp dụng.
   - Idempotent: chạy lại an toàn.
   ============================================================ */

-- 1) Thêm các cột model yêu cầu nếu còn thiếu
IF COL_LENGTH('dbo.blog_post','status')        IS NULL ALTER TABLE dbo.blog_post ADD [status] INT NULL;
IF COL_LENGTH('dbo.blog_post','is_deleted')    IS NULL ALTER TABLE dbo.blog_post ADD [is_deleted] BIT NULL CONSTRAINT DF_blog_post_is_deleted DEFAULT(0);
IF COL_LENGTH('dbo.blog_post','approver_id')   IS NULL ALTER TABLE dbo.blog_post ADD [approver_id] INT NULL;
IF COL_LENGTH('dbo.blog_post','approved_at')   IS NULL ALTER TABLE dbo.blog_post ADD [approved_at] DATETIME NULL;
IF COL_LENGTH('dbo.blog_post','reject_reason') IS NULL ALTER TABLE dbo.blog_post ADD [reject_reason] NVARCHAR(MAX) NULL;
IF COL_LENGTH('dbo.blog_post','rejected_at')   IS NULL ALTER TABLE dbo.blog_post ADD [rejected_at] DATETIME NULL;
IF COL_LENGTH('dbo.blog_post','updated_at')    IS NULL ALTER TABLE dbo.blog_post ADD [updated_at] DATETIME NULL;
IF COL_LENGTH('dbo.blog_post','tags')          IS NULL ALTER TABLE dbo.blog_post ADD [tags] NVARCHAR(500) NULL;
GO

-- 2) FK approver_id -> admin (đúng tên migration tạo ra ở bước cuối)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_blog_post_admin_approver_id')
   AND COL_LENGTH('dbo.blog_post','approver_id') IS NOT NULL
BEGIN
    ALTER TABLE dbo.blog_post
        ADD CONSTRAINT FK_blog_post_admin_approver_id
        FOREIGN KEY (approver_id) REFERENCES dbo.admin(admin_id);
    PRINT N'Added FK_blog_post_admin_approver_id';
END
GO

-- 3) Baseline migration: đánh dấu đã áp dụng để `dotnet ef database update` không chạy lại
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory
               WHERE MigrationId = '20260625100316_AddTagsToBlogPost')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260625100316_AddTagsToBlogPost', '8.0.1');
    PRINT N'Baselined migration 20260625100316_AddTagsToBlogPost';
END
ELSE
    PRINT N'Migration already baselined';
GO
