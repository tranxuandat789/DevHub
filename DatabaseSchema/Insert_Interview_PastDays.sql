USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
/* ============================================================
 Chen interview m?u cho cac application APPROVED (ch?a co interview),
 r?i created_at v? nh?ng ngay TR??C (trong 30 ngay qua) ?? dashboard
 co d? li?u ???ng "L??t ph?ng v?n".
 Idempotent: b? qua application ?a co interview.
 ============================================================ */
;
WITH ApprovedApps AS (
    SELECT a.application_id,
        a.candidate_id,
        r.recruiter_id,
        j.title,
        ROW_NUMBER() OVER (
            ORDER BY a.application_id
        ) AS rn
    FROM dbo.application a
        JOIN dbo.job_post j ON j.job_id = a.job_id
        JOIN dbo.recruiter r ON r.company_id = j.company_id
    WHERE UPPER(a.status) = 'APPROVED'
        AND NOT EXISTS (
            SELECT 1
            FROM dbo.interview i
            WHERE i.application_id = a.application_id
        )
)
INSERT INTO dbo.interview (
        application_id,
        candidate_id,
        scheduled_time,
        meeting_link,
        location,
        status,
        notes,
        created_at,
        updated_at
    )
SELECT application_id,
    candidate_id,
    DATEADD(DAY, -(rn * 2) + 2, GETDATE()),
    -- l?ch PV (lui theo t?ng m?c)
    N'https://meet.google.com/abc-defg-hij',
    N'Phong h?p t?ng 5 - V?n phong cong ty',
    CASE
        WHEN rn % 3 = 0 THEN'FINISHED'
        ELSE 'SCHEDULED'
    END,
    N'Ph?ng v?n vong 1: ' + ISNULL(title, N''),
    DATEADD(DAY, -(rn * 2), GETDATE()),
    -- created_at: r?i m?i 2 ngay lui v? qua kh?
    DATEADD(DAY, -(rn * 2), GETDATE())
FROM ApprovedApps
WHERE rn <= 14;
-- t?i ?a 14 m?c (~28 ngay)
PRINT N'?a cheN' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' interview (r?i cac ngay tr??c).';
GO -- Ki?m tra phan b? theo ngay
SELECT CONVERT(varchar, created_at, 23) AS ngay_tao,
    COUNT(*) AS so_interview
FROM dbo.interview
GROUP BY CONVERT(varchar, created_at, 23)
ORDER BY ngay_tao;
GO

