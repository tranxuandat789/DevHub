/* =============================================================================
   Migrate_JobPost_Salary_Province.sql
   AnhPT - JobPost Salary & Province migration (adapted from JobMatchingMigration.md)

   What it does (idempotent, transactional):
     1. Create [province] table + seed 63 provinces.
     2. Create [job_post_province] many-to-many junction (+ index, cascade FKs).
     3. Add [job_post].[salary_type] column (default 'RANGE') and backfill it
        from the existing salary_min / salary_max values.
     4. Backfill [job_post_province] from the old [job_post].[location] text
        via LIKE matching against province names.
     5. Drop the old [job_post].[location] column.

   Run against the LOCAL dev database only. The DROP of [location] is
   irreversible — make sure code (model/DbContext/views) is updated first.
   ============================================================================= */
SET XACT_ABORT ON;
SET NOCOUNT ON;
BEGIN TRANSACTION;

/* ---------------------------------------------------------------------------
   1. province table
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.province', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.province
    (
        province_id   INT IDENTITY(1,1) NOT NULL,
        province_name NVARCHAR(100)     NOT NULL,
        CONSTRAINT PK_province PRIMARY KEY (province_id),
        CONSTRAINT UQ_province_name UNIQUE (province_name)
    );
END;

/* Seed provinces (only those not already present). */
;WITH seed(province_name) AS (
    SELECT v FROM (VALUES
        (N'Hà Nội'),(N'Hồ Chí Minh'),(N'Hải Phòng'),(N'Đà Nẵng'),(N'Cần Thơ'),
        (N'An Giang'),(N'Bà Rịa - Vũng Tàu'),(N'Bắc Giang'),(N'Bắc Kạn'),(N'Bạc Liêu'),
        (N'Bắc Ninh'),(N'Bến Tre'),(N'Bình Định'),(N'Bình Dương'),(N'Bình Phước'),
        (N'Bình Thuận'),(N'Cà Mau'),(N'Cao Bằng'),(N'Đắk Lắk'),(N'Đắk Nông'),
        (N'Điện Biên'),(N'Đồng Nai'),(N'Đồng Tháp'),(N'Gia Lai'),(N'Hà Giang'),
        (N'Hà Nam'),(N'Hà Tĩnh'),(N'Hải Dương'),(N'Hậu Giang'),(N'Hòa Bình'),
        (N'Hưng Yên'),(N'Khánh Hòa'),(N'Kiên Giang'),(N'Kon Tum'),(N'Lai Châu'),
        (N'Lâm Đồng'),(N'Lạng Sơn'),(N'Lào Cai'),(N'Long An'),(N'Nam Định'),
        (N'Nghệ An'),(N'Ninh Bình'),(N'Ninh Thuận'),(N'Phú Thọ'),(N'Phú Yên'),
        (N'Quảng Bình'),(N'Quảng Nam'),(N'Quảng Ngãi'),(N'Quảng Ninh'),(N'Quảng Trị'),
        (N'Sóc Trăng'),(N'Sơn La'),(N'Tây Ninh'),(N'Thái Bình'),(N'Thái Nguyên'),
        (N'Thanh Hóa'),(N'Thừa Thiên Huế'),(N'Tiền Giang'),(N'Trà Vinh'),(N'Tuyên Quang'),
        (N'Vĩnh Long'),(N'Vĩnh Phúc'),(N'Yên Bái')
    ) AS t(v)
)
INSERT INTO dbo.province (province_name)
SELECT s.province_name
FROM seed s
WHERE NOT EXISTS (SELECT 1 FROM dbo.province p WHERE p.province_name = s.province_name);

/* ---------------------------------------------------------------------------
   2. job_post_province junction
   --------------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.job_post_province', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.job_post_province
    (
        job_id      INT NOT NULL,
        province_id INT NOT NULL,
        CONSTRAINT PK_job_post_province PRIMARY KEY (job_id, province_id),
        CONSTRAINT FK_job_post_province_job
            FOREIGN KEY (job_id) REFERENCES dbo.job_post (job_id) ON DELETE CASCADE,
        CONSTRAINT FK_job_post_province_province
            FOREIGN KEY (province_id) REFERENCES dbo.province (province_id) ON DELETE CASCADE
    );

    CREATE INDEX idx_job_post_province_province ON dbo.job_post_province (province_id);
END;

/* ---------------------------------------------------------------------------
   3. salary_type column on job_post (+ backfill from existing min/max)
   --------------------------------------------------------------------------- */
IF COL_LENGTH(N'dbo.job_post', N'salary_type') IS NULL
BEGIN
    ALTER TABLE dbo.job_post
        ADD salary_type NVARCHAR(20) NOT NULL CONSTRAINT DF_job_post_salary_type DEFAULT (N'RANGE');
END;

/* Backfill salary_type based on which bounds are present. Wrapped in EXEC so the
   newly-added column is resolvable without a GO batch separator (keeps the whole
   migration inside one transaction). */
EXEC(N'
    UPDATE dbo.job_post
    SET salary_type =
        CASE
            WHEN salary_min IS NULL AND salary_max IS NULL THEN N''NEGOTIABLE''
            WHEN salary_min IS NOT NULL AND salary_max IS NULL THEN N''FROM''
            WHEN salary_min IS NULL AND salary_max IS NOT NULL THEN N''UPTO''
            ELSE N''RANGE''
        END;
');

/* ---------------------------------------------------------------------------
   4. Backfill job_post_province from the old location text (LIKE matching).
      Only runs while the location column still exists.
   --------------------------------------------------------------------------- */
IF COL_LENGTH(N'dbo.job_post', N'location') IS NOT NULL
BEGIN
    INSERT INTO dbo.job_post_province (job_id, province_id)
    SELECT DISTINCT j.job_id, p.province_id
    FROM dbo.job_post j
    INNER JOIN dbo.province p
        ON j.location LIKE N'%' + p.province_name + N'%'
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.job_post_province jp
        WHERE jp.job_id = j.job_id AND jp.province_id = p.province_id
    );

    /* ----------------------------------------------------------------------
       5. Drop the old location column (and its EF default/index if any).
       ---------------------------------------------------------------------- */
    ALTER TABLE dbo.job_post DROP COLUMN location;
END;

COMMIT TRANSACTION;

/* ---- Verification (run after commit) ----
SELECT COUNT(*) AS provinces FROM dbo.province;
SELECT COUNT(*) AS links     FROM dbo.job_post_province;
SELECT COUNT(*) AS jobs_with_no_province
FROM dbo.job_post j
WHERE NOT EXISTS (SELECT 1 FROM dbo.job_post_province jp WHERE jp.job_id = j.job_id);
SELECT salary_type, COUNT(*) FROM dbo.job_post GROUP BY salary_type;
*/
