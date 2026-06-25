USE DevHub;
GO

-- 1. Update candidate 23 (đạt trần)
UPDATE candidate
SET experience_years = 2,
    expected_salary_min = 1000,
    expected_salary_max = 2000,
    preferred_location = N'Hà Nội',
    preferred_working_model = N'Remote'
WHERE candidate_id = 23;

-- Insert candidate skills
DELETE FROM candidate_skill WHERE candidate_id = 23;
INSERT INTO candidate_skill (candidate_id, tech_id) VALUES (23, 7); -- ReactJS
INSERT INTO candidate_skill (candidate_id, tech_id) VALUES (23, 4); -- C#
INSERT INTO candidate_skill (candidate_id, tech_id) VALUES (23, 17); -- SQL Server

-- 2. Insert Job Posts
INSERT INTO job_post (recruiter_id, position_id, recruiter_package_history_id, title, description, requirement, benefit, location, working_model, experience_level, salary_min, salary_max, status, created_at, deadline)
VALUES 
(4, 13, 1, N'Senior ReactJS Developer', N'We are looking for a Senior ReactJS Developer...', N'ReactJS, HTML, CSS', N'13th month salary', N'Hà Nội', N'Remote', N'Senior', 1500, 2500, N'APPROVED', GETDATE(), DATEADD(day, 30, GETDATE())),
(4, 13, 1, N'Fullstack .NET & ReactJS', N'Join our amazing team to build awesome web applications using .NET and ReactJS.', N'C#, ReactJS, SQL Server', N'Health insurance', N'Hà Nội', N'Fulltime', N'Middle', 1000, 1800, N'APPROVED', GETDATE(), DATEADD(day, 30, GETDATE())),
(4, 13, 1, N'Backend Engineer (C#)', N'Looking for a robust Backend Engineer', N'C#, .NET, SQL Server', N'Flexible working hours', N'Hà Nội', N'Remote', N'Junior', 800, 1500, N'APPROVED', GETDATE(), DATEADD(day, 30, GETDATE()));

-- Get the inserted job IDs (Top 3 ordered by job_id desc)
DECLARE @Job1 INT = (SELECT MAX(job_id) FROM job_post);
DECLARE @Job2 INT = @Job1 - 1;
DECLARE @Job3 INT = @Job1 - 2;

-- Insert Job Techs
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job1, 7);
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job2, 4);
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job2, 7);
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job2, 17);
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job3, 4);
INSERT INTO job_tech_stack (job_id, tech_id) VALUES (@Job3, 17);
GO
