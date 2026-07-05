USE DevHub;
GO
SET NOCOUNT ON;
/* ============================================================
 INSERT APPLICATIONS + CV (for testing)
 - Chỉ cho job_post status IN ('APPROVED','CLOSED') của recruiter 4 & 5.
 - Mỗi job gán 3 ứng viên (xoay vòng), mỗi application có 1 CV riêng
 (đúng candidate) trỏ tới 1 trong 5 file thật trong wwwroot/uploads/cvs.
 - 5 CV được CHIA ĐỀU round-robin theo từng application (arn % 5).
 - Idempotent: reset dữ liệu test cũ trước khi chèn lại.
 ============================================================ */
-- ============================================================
-- 0. RESET dữ liệu test cũ (để chạy lại sạch, không trùng)
-- ============================================================
DELETE a
FROM dbo.application a
  JOIN dbo.job_post j ON j.job_id = a.job_id
WHERE j.company_id IN (4, 5)
  AND j.status IN ('APPROVED', 'CLOSED');
-- Xóa các CV test không còn application nào tham chiếu
DELETE FROM dbo.cv
WHERE title LIKE N'[TEST]%'
  AND cv_id NOT IN (
    SELECT cv_id
    FROM dbo.application
  );
-- ============================================================
-- 1. Dựng bảng map: 1 dòng cho mỗi application (job × candidate)
--    kèm arn (số thứ tự toàn cục) để chia đều 5 file CV.
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
) -- 3 ứng viên / job
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
  -- để chia đều CV
  CAST(NULL AS NVARCHAR(255)) AS cv_title,
  CAST(NULL AS NVARCHAR(500)) AS cv_url INTO #Map
FROM Jobs j
  CROSS JOIN Slots s
  JOIN Cands c ON c.crn = (j.jrn + s.slot) % c.cnt;
-- Gán 5 file CV thật (chia đều theo arn % 5) + title duy nhất theo (job, candidate)
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
-- 2. Insert CV cho từng application (gắn đúng candidate)
-- ============================================================
INSERT INTO dbo.cv (candidate_id, title, cv_url, skills, is_default)
SELECT m.candidate_id,
  m.cv_title,
  m.cv_url,
  N'Kỹ năng theo hồ sơ ứng viên',
  0
FROM #Map m;
  PRINT N'Đã chèn CV test (5 file chia đều).';
-- ============================================================
-- 3. Insert application, tham chiếu đúng CV vừa tạo
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
  N'Tôi rất quan tâm tới vị trí này và mong muốn được đóng góp cho công ty.',
  m.app_status,
  DATEADD(
    DAY,
    -(m.slot + 1),
    ISNULL(m.approved_at, m.created_at)
  )
FROM #Map m
  JOIN dbo.cv cv ON cv.candidate_id = m.candidate_id
  AND cv.title = m.cv_title;
PRINT N'Đã chèn application cho job APPROVED/CLOSED của recruiter 4 & 5.';
-- ============================================================
-- 4. Đồng bộ application_count
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
PRINT N'Hoàn tất.';
GO -- ============================================================
  -- 5. Kiểm tra: phân bố 5 file CV trên các application
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
