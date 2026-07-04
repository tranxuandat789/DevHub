USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
/* ============================================================
 Chèn 1 Interview mẫu cho 1 application đang APPROVED
 (chưa có interview). recruiter_id lấy từ job của application.
 Idempotent: nếu application đó đã có interview thì bỏ qua.
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
IF @appId IS NULL BEGIN PRINT N'Không tìm thấy application APPROVED nào (chưa có interview).';
END
ELSE BEGIN
INSERT INTO dbo.interview (
        application_id,
        recruiter_id,
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
        @recruiterId,
        @candidateId,
        DATEADD(DAY, 3, GETDATE()),
        -- lịch sau 3 ngày (>= 24h)
        N'https://meet.google.com/abc-defg-hij',
        N'Phòng họp tầng 5 - Văn phòng công ty',
        'SCHEDULED',
        N'Phỏng vấn vòng 1 cho vị trí: ' + ISNULL(@jobTitle, N''),
        GETDATE(),
        GETDATE()
    );
PRINT N'Đã chèn interview cho application_id = ' + CAST(@appId AS NVARCHAR(10));
END
GO