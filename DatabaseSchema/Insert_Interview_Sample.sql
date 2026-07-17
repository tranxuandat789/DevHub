USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
/* ============================================================
 Chen 1 Interview m?u cho 1 application ?ang APPROVED
 (ch?a co interview). recruiter_id l?y t? job c?a application.
 Idempotent: n?u application ?o ?a co interview thi b? qua.
 ============================================================ */
DECLARE @appId INT,
    @candidateId INT,
    @recruiterId INT,
    @jobTitle NVARCHAR(255);
SELECT TOP 1 @appId = a.application_id,
    @candidateId = a.candidate_id,
    @recruiterId = r.recruiter_id,
    @jobTitle = j.title
FROM dbo.application a
    JOIN dbo.job_post j ON j.job_id = a.job_id
    JOIN dbo.recruiter r ON r.company_id = j.company_id
WHERE UPPER(a.status) = 'APPROVED'
    AND NOT EXISTS (
        SELECT 1
        FROM dbo.interview i
        WHERE i.application_id = a.application_id
    )
ORDER BY a.application_id;
IF @appId IS NULL BEGIN PRINT N'Khong tim th?y application APPROVED nao (ch?a co interview).';
END
ELSE BEGIN
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
VALUES (
        @appId,
        @candidateId,
        DATEADD(DAY, 3, GETDATE()),
        -- l?ch sau 3 ngay (>= 24h)
        N'https://meet.google.com/abc-defg-hij',
        N'Phong h?p t?ng 5 - V?n phong cong ty',
        'SCHEDULED',
        N'Ph?ng v?n vong 1 cho v? tri: ' + ISNULL(@jobTitle, N''),
        GETDATE(),
        GETDATE()
    );
PRINT N'?a chen interview cho application_id = ' + CAST(@appId AS NVARCHAR(10));
END
GO

