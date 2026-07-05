DECLARE @CandidateEmail NVARCHAR(255) = 'trandat14072005@gmail.com';
DECLARE @CandidateId INT;
DECLARE @JobId INT;
DECLARE @CvId INT;
DECLARE @ApplicationId INT;
DECLARE @RecruiterId INT;
DECLARE @CompanyId INT;

-- 1. Tìm ứng viên dựa trên Email
SELECT @CandidateId = user_id FROM user_account WHERE email = @CandidateEmail;

IF @CandidateId IS NULL
BEGIN
    PRINT N'Không tìm thấy ứng viên với email: ' + @CandidateEmail;
    RETURN;
END

-- Đảm bảo có record trong bảng candidate
IF NOT EXISTS (SELECT 1 FROM candidate WHERE candidate_id = @CandidateId)
BEGIN
    INSERT INTO candidate (candidate_id, full_name, profile_completion) 
    VALUES (@CandidateId, N'Trần Đạt', 100);
END

-- 2. Lấy CV của ứng viên (nếu chưa có thì insert tạm 1 CV để ứng tuyển)
SELECT TOP 1 @CvId = cv_id FROM cv WHERE candidate_id = @CandidateId ORDER BY is_default DESC;

IF @CvId IS NULL
BEGIN
    INSERT INTO cv (candidate_id, title, cv_url, is_default, created_at, updated_at)
    VALUES (@CandidateId, N'CV Ứng Tuyển', 'mock_cv.pdf', 1, GETDATE(), GETDATE());
    
    SET @CvId = SCOPE_IDENTITY();
END

-- 3. Chọn 1 công việc bất kỳ đang có trong hệ thống (Ưu tiên các job mới)
SELECT TOP 1 
    @JobId = job_id, 
    @CompanyId = company_id 
FROM job_post 
ORDER BY job_id DESC;

IF @JobId IS NULL
BEGIN
    PRINT N'Chưa có công việc nào trong database để ứng tuyển.';
    RETURN;
END

-- Xóa ứng tuyển cũ nếu đã ứng tuyển vào job này để tránh lỗi trùng lặp
IF EXISTS (SELECT 1 FROM application WHERE candidate_id = @CandidateId AND job_id = @JobId)
BEGIN
    DELETE FROM interview WHERE application_id IN (SELECT application_id FROM application WHERE candidate_id = @CandidateId AND job_id = @JobId);
    DELETE FROM application WHERE candidate_id = @CandidateId AND job_id = @JobId;
END

-- 4. Tạo Application với trạng thái APPROVED (đã duyệt/xác nhận)
INSERT INTO application (candidate_id, job_id, cv_id, status, applied_at)
VALUES (@CandidateId, @JobId, @CvId, 'APPROVED', GETDATE());

SET @ApplicationId = SCOPE_IDENTITY();

-- 5. Tìm 1 Recruiter của công ty đó để setup lịch phỏng vấn
SELECT TOP 1 @RecruiterId = recruiter_id FROM recruiter WHERE company_id = @CompanyId;

IF @RecruiterId IS NULL
BEGIN
    -- Nếu công ty không có recruiter, mượn tạm 1 recruiter bất kỳ trong hệ thống
    SELECT TOP 1 @RecruiterId = recruiter_id FROM recruiter;
END

-- 6. Tạo Interview (Lịch phỏng vấn)
IF @RecruiterId IS NOT NULL
BEGIN
    INSERT INTO interview (
        application_id, candidate_id, recruiter_id, 
        scheduled_time, location, meeting_link, 
        status, created_at, updated_at, interview_type
    )
    VALUES (
        @ApplicationId, @CandidateId, @RecruiterId, 
        DATEADD(day, 2, GETDATE()), -- Hẹn phỏng vấn vào 2 ngày sau
        N'Tầng 3, Tòa nhà DevHub', 
        NULL, -- Giả sử đây là buổi phỏng vấn offline 100%, không có link meeting
        'scheduled', GETDATE(), GETDATE(),
        N'Trực tiếp (Offline)' -- Chỉ định rõ hình thức
    );
    
    PRINT N'Đã tạo thành công Application (APPROVED) và Interview (Offline)!';
END
ELSE
BEGIN
    PRINT N'Đã tạo Application nhưng không tìm thấy Recruiter nào để tạo Interview.';
END
