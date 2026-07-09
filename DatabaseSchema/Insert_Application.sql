USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
/* ============================================================
 INSERT APPLICATIONS + CV (for testing)
 - Ch? cho job_post status IN ('APPROVED','CLOSED') c?a recruiter 4 & 5.
 - M?i job gan 3 ?ng vien (xoay vong), m?i application co 1 CV rieng
 (?ung candidate) tr? t?i 1 trong 5 file th?t trong wwwroot/uploads/cvs.
 - 5 CV ???c CHIA ??U round-robin theo t?ng application (arn % 5).
 - Idempotent: reset d? li?u test c? tr??c khi chen l?i.
 ============================================================ */
-- ============================================================
-- 0. RESET d? li?u test c? (?? ch?y l?i s?ch, khong trung)
-- ============================================================
DELETE a
FROM dbo.application a
  JOIN dbo.job_post j ON j.job_id = a.job_id
WHERE j.company_id IN (4, 5)
  AND j.status IN ('APPROVED', 'CLOSED');
-- Xoa cac CV test khong con application nao tham chi?u
DELETE FROM dbo.cv
WHERE title LIKE N'[TEST]%'
  AND cv_id NOT IN (
    SELECT cv_id
    FROM dbo.application
  );
-- ============================================================
-- 1. D?ng b?ng map: 1 dong cho m?i application (job Å~ candidate)
--    kem arn (s? th? t? toan c?c) ?? chia ??u 5 file CV.
-- ============================================================
IF OBJECT_ID('tempdb..#Map') IS NOT NULL DROP TABLE #Map;
;
WITH Jobs AS (
  SELECT job_id,
    status,
    approved_at,
    created_at,
    ROW_NUMBER() OVER (
      ORDER BY job_id
    ) - 1 AS jrn
  FROM dbo.job_post
  WHERE company_id IN (4, 5)
    AND status IN ('APPROVED', 'CLOSED')
),
Cands AS (
  SELECT candidate_id,
    ROW_NUMBER() OVER (
      ORDER BY candidate_id
    ) - 1 AS crn,
    COUNT(*) OVER () AS cnt
  FROM dbo.candidate
),
Slots AS (
  SELECT 0 AS slot
  UNION ALL
  SELECT 1
  UNION ALL
  SELECT 2
) -- 3 ?ng vien / job
SELECT j.job_id,
  c.candidate_id,
  j.approved_at,
  j.created_at,
  s.slot,
  CASE
    WHEN j.status = 'APPROVED' THEN CASE
      s.slot
      WHEN 0 THEN'PENDING'
      WHEN 1 THEN'APPROVED'
      ELSE 'REJECTED'
    END
    ELSE CASE
      s.slot
      WHEN 0 THEN'HIRED'
      WHEN 1 THEN'FINISHED'
      ELSE 'REJECTED'
    END
  END AS app_status,
  ROW_NUMBER() OVER (
    ORDER BY j.job_id,
      s.slot
  ) - 1 AS arn,
  -- ?? chia ??u CV
  CAST(NULL AS NVARCHAR(255)) AS cv_title,
  CAST(NULL AS NVARCHAR(500)) AS cv_url INTO #Map
FROM Jobs j
  CROSS JOIN Slots s
  JOIN Cands c ON c.crn = (j.jrn + s.slot) % c.cnt;
-- Gan 5 file CV th?t (chia ??u theo arn % 5) + title duy nh?t theo (job, candidate)
UPDATE m
SET cv_title = N'[TEST] CV J' + CAST(m.job_id AS NVARCHAR(10)) + N'-C' + CAST(m.candidate_id AS NVARCHAR(10)),
  cv_url = f.url
FROM #Map m
  JOIN (
    VALUES (
        0,
        N'/uploads/cvs/1_633d6964-5424-4f62-ae1c-f026d9c518ad.docx'
      ),
      (
        1,
        N'/uploads/cvs/23_5486f90b-dcfe-436e-8012-84348cbd697f.docx'
      ),
      (
        2,
        N'/uploads/cvs/23_88027a26-c81a-4c0d-a981-efb1a50d826b.pdf'
      ),
      (
        3,
        N'/uploads/cvs/23_e8626748-26b4-4859-9af0-5c183050a74f.pdf'
      ),
      (
        4,
        N'/uploads/cvs/24_52e163f8-d099-4fc4-81c7-a57f19fe27c9.docx'
      )
  ) AS f(idx, url) ON f.idx = m.arn % 5;
-- ============================================================
-- 2. Insert CV cho t?ng application (g?n ?ung candidate)
-- ============================================================
INSERT INTO dbo.cv (candidate_id, title, cv_url, skills, is_default)
SELECT m.candidate_id,
  m.cv_title,
  m.cv_url,
  N'K? n?ng theo h? s? ?ng vien',
  0
FROM #Map m;
  PRINT N'?a chen CV test (5 file chia ??u).';
-- ============================================================
-- 3. Insert application, tham chi?u ?ung CV v?a t?o
-- ============================================================
INSERT INTO dbo.application (
    job_id,
    candidate_id,
    cv_id,
    cover_letter,
    status,
    applied_at
  )
SELECT m.job_id,
  m.candidate_id,
  cv.cv_id,
  N'Toi r?t quan tam t?i v? tri nay va mong mu?n ???c ?ong gop cho cong ty.',
  m.app_status,
  DATEADD(
    DAY,
    -(m.slot + 1),
    ISNULL(m.approved_at, m.created_at)
  )
FROM #Map m
  JOIN dbo.cv cv ON cv.candidate_id = m.candidate_id
  AND cv.title = m.cv_title;
PRINT N'?a chen application cho job APPROVED/CLOSED c?a recruiter 4 & 5.';
-- ============================================================
-- 4. ??ng b? application_count
-- ============================================================
UPDATE jp
SET jp.application_count = x.cnt
FROM dbo.job_post jp
  JOIN (
    SELECT job_id,
      COUNT(*) AS cnt
    FROM dbo.application
    GROUP BY job_id
  ) x ON x.job_id = jp.job_id
WHERE jp.company_id IN (4, 5)
  AND jp.status IN ('APPROVED', 'CLOSED');
DROP TABLE #Map;
PRINT N'Hoan t?t.';
GO -- ============================================================
  -- 5. Ki?m tra: phan b? 5 file CV tren cac application
  -- ============================================================
SELECT cv.cv_url,
  COUNT(*) AS total_applications
FROM dbo.application a
  JOIN dbo.job_post j ON j.job_id = a.job_id
  JOIN dbo.cv cv ON cv.cv_id = a.cv_id
WHERE j.company_id IN (4, 5)
  AND j.status IN ('APPROVED', 'CLOSED')
GROUP BY cv.cv_url
ORDER BY cv.cv_url;
GO

