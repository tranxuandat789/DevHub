USE ITRecruitmentDB;
GO

SET NOCOUNT ON;

/* ============================================================
   Make blog_post.publisher_id NULLable (khớp Models/BlogPost.cs: int? PublisherId).
   Đổi NOT NULL -> NULL không cần drop FK/index. Idempotent.
   ============================================================ */
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'blog_post' AND COLUMN_NAME = 'publisher_id' AND IS_NULLABLE = 'NO')
BEGIN
    ALTER TABLE dbo.blog_post ALTER COLUMN [publisher_id] INT NULL;
    PRINT N'blog_post.publisher_id is now NULLable';
END
ELSE
    PRINT N'blog_post.publisher_id is already NULLable';
GO
