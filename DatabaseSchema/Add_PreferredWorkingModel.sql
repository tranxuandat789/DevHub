USE DevHub;
GO

/* ============================================================
   Migrate: candidate.preferred_working_model (from friend's branch)
   - DB hiện tại tạo từ SQL script => migration "AddPreferredWorkingModel"
     là initial create-all, KHÔNG chạy `dotnet ef database update` được
     (sẽ lỗi vì các bảng đã tồn tại).
   - Script này: (1) thêm đúng cột mới, (2) baseline migration vào
     __EFMigrationsHistory để EF coi như đã áp dụng.
   - Idempotent: chạy lại an toàn.
   ============================================================ */

-- 1) Thêm cột mới cho bảng candidate
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'candidate' AND COLUMN_NAME = 'preferred_working_model')
BEGIN
    ALTER TABLE dbo.candidate ADD preferred_working_model NVARCHAR(50) NULL;
    PRINT N'Added column candidate.preferred_working_model';
END
ELSE
    PRINT N'Column candidate.preferred_working_model already exists';
GO

-- 2) Baseline migration: đánh dấu migration đã áp dụng để các lần
--    `dotnet ef database update` sau KHÔNG cố tạo lại bảng đã tồn tại.
IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
END

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory
               WHERE MigrationId = '20260616042906_AddPreferredWorkingModel')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260616042906_AddPreferredWorkingModel', '8.0.1');
    PRINT N'Baselined migration 20260616042906_AddPreferredWorkingModel';
END
ELSE
    PRINT N'Migration already baselined';
GO
