-- =====================================================
-- Bổ sung 4 trường Thuế/Hóa đơn vào package_transaction
-- Tác giả: Agent
-- Ngày: 09/07/2026
-- =====================================================
USE DevHub;
GO

IF COL_LENGTH('package_transaction', 'vat_rate') IS NULL
BEGIN
    ALTER TABLE [package_transaction] ADD
        [vat_rate] DECIMAL(5,2) NOT NULL DEFAULT 8,
        [vat_amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [total_amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [buyer_tax_code] NVARCHAR(50) NULL;
    PRINT N'Added VAT columns to package_transaction';
END
ELSE
BEGIN
    PRINT N'VAT columns already exist in package_transaction';
END
GO

-- =====================================================
-- Baseline migration: đánh dấu migration AddVATColumns
-- để Entity Framework không chạy lại lệnh ALTER bên trên.
-- =====================================================
IF OBJECT_ID(N'__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260709033438_AddVATColumns')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260709033438_AddVATColumns', '8.0.6'); -- Thay version tùy ý, EF Core 8.0.x
    PRINT N'Baselined migration 20260709033438_AddVATColumns';
END
GO
