USE ITRecruitmentDB;
GO

/* ============================================================
   Migrate: candidate.preferred_working_model (from friend's branch)
   - DB hi?n t?i t?o t? SQL script => migration "AddPreferredWorkingModel"
     la initial create-all, KHONG ch?y `dotnet ef database update` ???c
     (s? l?i vi cac b?ng ?a t?n t?i).
   - Script nay: (1) them ?ung c?t m?i, (2) baseline migration vao
     __EFMigrationsHistory ?? EF coi nh? ?a ap d?ng.
   - Idempotent: ch?y l?i an toan.
   ============================================================ */

-- 1) Them c?t m?i cho b?ng candidate
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

-- 2) Baseline migration: ?anh d?u migration ?a ap d?ng ?? cac l?n
--    `dotnet ef database update` sau KHONG c? t?o l?i b?ng ?a t?n t?i.
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

