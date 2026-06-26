USE ITRecruitmentDB;
GO

SET NOCOUNT ON;

/* ============================================================
   Chèn interview mẫu cho các application APPROVED (chưa có interview),
   rải created_at về những ngày TRƯỚC (trong 30 ngày qua) để dashboard
   có dữ liệu đường "Lượt phỏng vấn".
   Idempotent: bỏ qua application đã có interview.
   ============================================================ */
;WITH ApprovedApps AS (
    SELECT  a.application_id,
            a.candidate_id,
            j.recruiter_id,
            j.title,
            ROW_NUMBER() OVER (ORDER BY a.application_id) AS rn
    FROM    dbo.application a
    JOIN    dbo.job_post   j ON j.job_id = a.job_id
    WHERE   UPPER(a.status) = 'APPROVED'
      AND   NOT EXISTS (SELECT 1 FROM dbo.interview i WHERE i.application_id = a.application_id)
)
INSERT INTO dbo.interview
    (application_id, recruiter_id, candidate_id, scheduled_time, meeting_link, location, status, notes, created_at, updated_at)
SELECT
    application_id,
    recruiter_id,
    candidate_id,
    DATEADD(DAY, -(rn * 2) + 2, GETDATE()),               -- lịch PV (lùi theo từng mốc)
    N'https://meet.google.com/abc-defg-hij',
    N'Phòng họp tầng 5 - Văn phòng công ty',
    CASE WHEN rn % 3 = 0 THEN 'FINISHED' ELSE 'SCHEDULED' END,
    N'Phỏng vấn vòng 1: ' + ISNULL(title, N''),
    DATEADD(DAY, -(rn * 2), GETDATE()),                   -- created_at: rải mỗi 2 ngày lùi về quá khứ
    DATEADD(DAY, -(rn * 2), GETDATE())
FROM    ApprovedApps
WHERE   rn <= 14;                                          -- tối đa 14 mốc (~28 ngày)

PRINT N'Đã chèn ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' interview (rải các ngày trước).';
GO

-- Kiểm tra phân bố theo ngày
SELECT  CONVERT(varchar, created_at, 23) AS ngay_tao, COUNT(*) AS so_interview
FROM    dbo.interview
GROUP BY CONVERT(varchar, created_at, 23)
ORDER BY ngay_tao;
GO
