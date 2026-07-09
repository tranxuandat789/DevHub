-- =====================================================
-- Add Moderator Task Type & Company Moderator Assignment
-- Author: Admin
-- Date: 07/07/2026
-- =====================================================
USE DevHub;

-- 1. Thêm cột moderator_id vào bảng company
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[company]') AND name = 'moderator_id'
)
BEGIN
    ALTER TABLE [company] ADD [moderator_id] INT NULL;
    ALTER TABLE [company] ADD CONSTRAINT [FK__company__moderator_id]
        FOREIGN KEY ([moderator_id]) REFERENCES [admin]([admin_id]) ON DELETE SET NULL;
    PRINT 'Added moderator_id column to company table.';
END
ELSE
BEGIN
    PRINT 'Column moderator_id already exists in company table.';
END

-- 2. Tạo bảng moderator_task_type
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[moderator_task_type]') AND type = 'U')
BEGIN
    CREATE TABLE [moderator_task_type] (
        [id]            INT PRIMARY KEY IDENTITY(1,1),
        [moderator_id]  INT NOT NULL UNIQUE,
        [task_type]     NVARCHAR(30) NOT NULL
                            CHECK ([task_type] IN ('COMPANY_APPROVAL', 'JOB_POST', 'REVIEW')),
        [assigned_by]   INT NOT NULL,
        [created_at]    DATETIME DEFAULT GETDATE(),
        [updated_at]    DATETIME DEFAULT GETDATE(),
        CONSTRAINT [FK__mod_task_type__moderator]
            FOREIGN KEY ([moderator_id]) REFERENCES [admin]([admin_id]) ON DELETE CASCADE,
        CONSTRAINT [FK__mod_task_type__assigned_by]
            FOREIGN KEY ([assigned_by]) REFERENCES [admin]([admin_id]) ON DELETE NO ACTION
    );
    PRINT 'Created table moderator_task_type.';
END
ELSE
BEGIN
    PRINT 'Table moderator_task_type already exists.';
END
