-- =======================================================================
-- MIGRATION: Thêm các cột rating chi tiết cho bảng review_company
-- =======================================================================

USE DevHub;
GO

-- Kiểm tra xem cột đã tồn tại chưa để tránh lỗi chạy lại
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[review_company]') 
      AND name = 'salary_rating'
)
BEGIN
    ALTER TABLE [review_company]
    ADD 
        [salary_rating]    INT NULL,
        [training_rating]  INT NULL,
        [care_rating]      INT NULL,
        [culture_rating]   INT NULL,
        [workspace_rating] INT NULL,
        [ot_policy]        NVARCHAR(20) NULL CHECK ([ot_policy] IN ('SATISFIED', 'UNSATISFIED')),
        [recommend]        BIT NULL;
        
    PRINT 'Đã thêm các cột rating chi tiết vào bảng review_company thành công.';
END
ELSE
BEGIN
    PRINT 'Các cột rating chi tiết đã tồn tại trong bảng review_company.';
END
GO
