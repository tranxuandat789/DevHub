USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
-- Khai bao c?u truc b?ng t?m ?? l?u l?i danh sach JobId t? ??ng sinh ra
DECLARE @InsertedJobs TABLE (
        RowNumber INT IDENTITY(1, 1),
        JobId INT,
        Status NVARCHAR(50)
    );
-- =============================================
-- 1. INSERT 20 RECORDS CHO B?NG job_post
-- =============================================
INSERT INTO dbo.job_post (
        company_id,
        position_id,
        company_package_history_id,
        title,
        description,
        requirement,
        benefit,
        skill,
        experience_level,
        working_model,
        salary_min,
        salary_max,
        hiring_quota,
        deadline,
        status,
        priority_score,
        application_count,
        approved_at,
        rejected_reason,
        created_at,
        moderator_id
    ) OUTPUT inserted.job_id,
    inserted.status INTO @InsertedJobs(JobId, Status)
VALUES -- 1. PENDING (Ch? duy?t: Khong co approved_at, khong co rejected_reason)
    (
        4,
        1,
        2,
        N'Senior .NET Core Developer',
        N'Phat tri?n h? th?ng microservices loi va t?i ?u truy v?n d? li?u l?n.',
        N'T?i thi?u 4 n?m kinh nghi?m v?i C# .NET Core, SQL Server.',
        N'L??ng thang 13, b?o hi?m s?c kh?e cao c?p ??c quy?n.',
        N'C#, .NET Core, SQL',
        N'Senior',
        N'Full-time',
        25000000,
        40000000,
        2,
        '2026-08-15',
        'PENDING',
        50,
        0,
        NULL,
        NULL,
        '2026-06-03 09:00:00',
        NULL
    ),
    -- 2. APPROVED (?ang ho?t ??ng: ?a duy?t sau khi t?o 4 ti?ng)
    (
        4,
        3,
        2,
        N'Frontend Angular Developer',
        N'Xay d?ng giao di?n Dashboard qu?n tr? h? th?ng phan tich d? li?u.',
        N'Kinh nghi?m th?c chi?n Angular 14+, TypeScript, RxJS.',
        N'Th??ng d? an, c?p Macbook Pro, ph? c?p ?n tr?a.',
        N'Angular, TypeScript, CSS',
        N'Mid-Level',
        N'Hybrid',
        18000000,
        28000000,
        3,
        '2026-07-20',
        'APPROVED',
        75,
        4,
        '2026-06-01 14:30:00',
        NULL,
        '2026-06-01 10:30:00',
        8
    ),
    -- 3. REJECTED (B? t? ch?i: Co ly do t? ch?i, khong co approved_at)
    (
        4,
        4,
        2,
        N'Fullstack Web NodeJS/React',
        N'Tham gia xay d?ng s?n ph?m E-Commerce t? giai ?o?n kh?i t?o.',
        N'Thanh th?o ReactJS va cac framework NodeJS (Express, NestJS).',
        N'Review l??ng 2 l?n/n?m, l? trinh th?ng ti?n ro rang.',
        N'ReactJS, NodeJS, MongoDB',
        N'Junior/Mid',
        N'Full-time',
        15000000,
        25000000,
        5,
        '2026-06-30',
        'REJECTED',
        30,
        0,
        NULL,
        N'N?i dung tin ch?a lien k?t ngoai h? th?ng va thong tin ca nhan sai quy ??nh.',
        '2026-06-02 08:00:00',
        8
    ),
    -- 4. CLOSED (?a ?ong th? cong/h?t h?n s?m: Co approved_at)
    (
        4,
        8,
        2,
        N'DevOps AWS Engineer',
        N'Thi?t l?p, t?i ?u va v?n hanh h? th?ng CI/CD tren n?n t?ng AWS Cloud.',
        N'Co ch?ng ch? AWS, kinh nghi?m sau v?i Docker, Kubernetes, Terraform.',
        N'Lam vi?c 5 ngay/tu?n, goi phuc l?i teambuilding hang quy.',
        N'AWS, Docker, CI/CD',
        N'Senior',
        N'Remote',
        35000000,
        55000000,
        1,
        '2026-07-01',
        'CLOSED',
        80,
        2,
        '2026-05-16 11:00:00',
        NULL,
        '2026-05-15 15:00:00',
        8
    ),
    -- 5. CLOSED (H?t h?n: Deadline TR??C 04/06/2026, co approved_at)
    (
        4,
        11,
        2,
        N'Automation Tester Specialist',
        N'Vi?t test script t? ??ng hoa ki?m th? cho cac s?n ph?m Web & Mobile.',
        N'Kinh nghi?m v?ng vang v?i Selenium WebDriver, Java ho?c Python.',
        N'L??ng thang 14, h? tr? chi phi thi cac ch?ng ch? qu?c t?.',
        N'Selenium, Java, Automation',
        N'Mid-Level',
        N'Full-time',
        16000000,
        26000000,
        2,
        '2026-05-25',
        'CLOSED',
        40,
        12,
        '2026-04-02 09:15:00',
        NULL,
        '2026-04-01 11:00:00',
        8
    ),
    -- 6. PENDING
    (
        4,
        5,
        2,
        N'iOS Swift Engineer',
        N'Phat tri?n ?ng d?ng di ??ng Native ?ap ?ng hang tri?u ng??i dung.',
        N'H?n 2 n?m lam vi?c chuyen sau v?i Swift, UIKit va SwiftUI.',
        N'Ph? c?p g?i xe, trang thi?t b? hi?n ??i, moi tr??ng tr? trung.',
        N'iOS, Swift, Xcode',
        N'Mid-Level',
        N'Full-time',
        20000000,
        32000000,
        2,
        '2026-07-15',
        'PENDING',
        60,
        0,
        NULL,
        NULL,
        '2026-06-03 16:20:00',
        NULL
    ),
    -- 7. APPROVED
    (
        4,
        14,
        2,
        N'IT Business Analyst (BA)',
        N'Kh?o sat yeu c?u t? khach hang doanh nghi?p, vi?t tai li?u SRS, User Story.',
        N'Kinh nghi?m BA tren 2 n?m trong l?nh v?c Fintech ho?c ERP.',
        N'C? h?i Onsite ng?n h?n, b?o hi?m s?c kh?e toan di?n.',
        N'BA, SQL, UML, Jira',
        N'Mid-Level',
        N'Full-time',
        18000000,
        30000000,
        2,
        '2026-07-05',
        'APPROVED',
        70,
        3,
        '2026-05-29 10:00:00',
        NULL,
        '2026-05-28 14:00:00',
        8
    ),
    -- 8. APPROVED
    (
        4,
        2,
        2,
        N'Junior Python Developer',
        N'Phat tri?n, b?o tri h? th?ng cao d? li?u l?n va cac cong c? Automation.',
        N'N?m ch?c Python c? b?n, OOP, c?u truc d? li?u, th? vi?n BS4/Scrapy.',
        N'???c ?ao t?o bai b?n b?i Mentor Senior giau kinh nghi?m.',
        N'Python, BeautifulSoup, SQL',
        N'Junior',
        N'Full-time',
        11000000,
        16000000,
        4,
        '2026-06-25',
        'APPROVED',
        50,
        15,
        '2026-05-26 09:00:00',
        NULL,
        '2026-05-25 17:30:00',
        8
    ),
    -- 9. REJECTED
    (
        4,
        18,
        2,
        N'Data Engineer (Data Pipeline)',
        N'Xay d?ng lu?ng x? ly va lam s?ch d? li?u l?n ph?c v? h? th?ng BI.',
        N'Kinh nghi?m xay d?ng ki?n truc d? li?u v?ng v?i Spark, Hadoop, SQL.',
        N'Moi tr??ng n?ng ??ng, review hi?u su?t cong vi?c ro rang.',
        N'Spark, Python, BigData',
        N'Senior',
        N'Hybrid',
        30000000,
        50000000,
        1,
        '2026-06-30',
        'REJECTED',
        85,
        0,
        NULL,
        N'Thong tin m?c l??ng khong ro rang, yeu c?u k? n?ng qua chung chung.',
        '2026-05-20 10:00:00',
        8
    ),
    -- 10. APPROVED
    (
        4,
        12,
        2,
        N'Manual QC Tester',
        N'L?p k? ho?ch test case, th?c hi?n test th? cong, qu?n ly theo doi bug.',
        N'Hi?u quy trinh phat hanh ph?n m?m, c?n th?n, ch?u ap l?c t?t.',
        N'Th??ng cac ngay l? t?t, c?p may tinh lam vi?c t?i v?n phong.',
        N'Manual Test, Testcase, Jira',
        N'Junior/Mid',
        N'Full-time',
        10000000,
        15000000,
        3,
        '2026-05-10',
        'APPROVED',
        30,
        9,
        '2026-05-11 13:00:00',
        NULL,
        '2026-06-10 15:45:00',
        8
    ),
    -- 11. PENDING
    (
        4,
        1,
        2,
        N'Golang Backend Engineer',
        N'Xay d?ng h? th?ng Core thanh toan th?i gian th?c ch?u t?i c?c l?n.',
        N'Kinh nghi?m l?p trinh Golang t?t ho?c v?ng ngon ng? h??ng ??i t??ng.',
        N'Moi tr??ng ?a qu?c gia, lam vi?c hoan toan b?ng ti?ng Anh.',
        N'Golang, Redis, Kafka',
        N'Mid-Level',
        N'Full-time',
        22000000,
        38000000,
        2,
        '2026-07-18',
        'PENDING',
        65,
        0,
        NULL,
        NULL,
        '2026-06-03 11:10:00',
        NULL
    ),
    -- 12. APPROVED
    (
        4,
        7,
        2,
        N'Flutter Mobile Developer',
        N'Phat tri?n ?ng d?ng m?ng xa h?i ch?y m??t ma ?a n?n t?ng.',
        N'H?n 1.5 n?m kinh nghi?m lam s?n ph?m th?c t? v?i Flutter & Dart.',
        N'H? tr? c?m tr?a, mi?n phi ?? ?n nh? t?i pantry cong ty.',
        N'Flutter, Dart, Bloc',
        N'Mid-Level',
        N'Hybrid',
        16000000,
        24000000,
        2,
        '2026-07-28',
        'APPROVED',
        60,
        5,
        '2026-05-21 16:00:00',
        NULL,
        '2026-05-20 13:20:00',
        8
    ),
    -- 13. APPROVED
    (
        4,
        3,
        2,
        N'Senior ReactJS Developer',
        N'Lam ch? ki?n truc giao di?n cac module loi tr?c thu?c h? sinh thai.',
        N'T?i thi?u 4 n?m kinh nghi?m Frontend, sau s?c v? NextJS, Redux Toolkit.',
        N'M?c l??ng ??t pha, thang l??ng th? 14 ?n ??nh h?ng n?m.',
        N'ReactJS, NextJS, Redux',
        N'Senior',
        N'Full-time',
        28000000,
        45000000,
        1,
        '2026-07-12',
        'APPROVED',
        90,
        8,
        '2026-05-18 10:15:00',
        NULL,
        '2026-05-17 11:00:00',
        8
    ),
    -- 14. REJECTED
    (
        4,
        19,
        2,
        N'AI / Machine Learning Engineer',
        N'Nghien c?u va tich h?p cac mo hinh Generative AI t?i ?u hoa s?n ph?m.',
        N'N?n t?ng toan t?i ?u v?ng, kinh nghi?m sau v?i PyTorch, TensorFlow.',
        N'Lam vi?c cung chuyen gia ??u nganh, l? trinh R&D ro rang.',
        N'AI, Python, PyTorch',
        N'Mid/Senior',
        N'Full-time',
        35000000,
        60000000,
        2,
        '2026-07-01',
        'REJECTED',
        95,
        0,
        NULL,
        N'Ten v? tri va mo t? cong vi?c b?ng ngon t? khong chu?n m?c l?ch s?.',
        '2026-05-25 14:00:00',
        8
    ),
    -- 15. CLOSED
    (
        4,
        13,
        2,
        N'Embedded Systems Engineer',
        N'L?p trinh firmware nhung, ?i?u khi?n vi x? ly cho thi?t b? IoT.',
        N'Thanh th?o C/C++, hi?u bi?t sau s?c v? ki?n truc ph?n c?ng ARM.',
        N'Tr? c?p ??c h?i phong lab, du l?ch ngh? d??ng cao c?p.',
        N'Embedded C, C++, IoT, ARM',
        N'Mid-Level',
        N'Full-time',
        20000000,
        32000000,
        1,
        '2026-06-30',
        'CLOSED',
        55,
        1,
        '2026-05-06 09:30:00',
        NULL,
        '2026-05-05 10:15:00',
        8
    ),
    -- 16. CLOSED
    (
        4,
        16,
        2,
        N'Technical Project Manager (IT PM)',
        N'Qu?n ly ti?n ?? d? an, ?i?u ph?i nhan s?, lam vi?c ch?t ch? v?i khach hang.',
        N'It nh?t 2 n?m lam PM, giao ti?p ti?ng Anh troi ch?y b?t bu?c.',
        N'Th??ng qu?n ly, c? ph?n th??ng h?p d?n tuy hi?u su?t.',
        N'PM, Agile, Scrum',
        N'Manager',
        N'Full-time',
        35000000,
        50000000,
        1,
        '2026-05-01',
        'CLOSED',
        80,
        14,
        '2026-03-16 11:00:00',
        NULL,
        '2026-03-15 14:30:00',
        8
    ),
    -- 17. PENDING
    (
        4,
        3,
        2,
        N'VueJS Frontend Developer',
        N'Nang c?p h? th?ng Web App SaaS sang ki?n truc Single Page.',
        N'Thanh th?o VueJS v3 (Composition API), Pinia, Tailwind CSS.',
        N'Moi tr??ng it OT, can b?ng t?t gi?a cong vi?c va ??i s?ng.',
        N'VueJS, Pinia, CSS',
        N'Junior/Mid',
        N'Full-time',
        13000000,
        20000000,
        2,
        '2026-07-25',
        'PENDING',
        45,
        0,
        NULL,
        NULL,
        '2026-06-03 15:45:00',
        NULL
    ),
    -- 18. APPROVED
    (
        4,
        1,
        2,
        N'Java Spring Boot Backend Engineer',
        N'Tai c?u truc h? th?ng Monolith c? sang mo hinh ki?n truc Microservices.',
        N'T?i thi?u 3 n?m kinh nghi?m v?i Java Core, Spring Boot, Hibernate.',
        N'Ph? c?p gym/yoga, tra ca phe mi?n phi khong gi?i h?n.',
        N'Java, Spring Boot, MySQL',
        N'Mid-Level',
        N'Full-time',
        17000000,
        27000000,
        3,
        '2026-07-10',
        'APPROVED',
        65,
        2,
        '2026-05-22 09:00:00',
        NULL,
        '2026-05-21 15:00:00',
        8
    ),
    -- 19. APPROVED
    (
        4,
        9,
        2,
        N'System Linux Administrator',
        N'Giam sat ho?t ??ng, b?o m?t va sao l?u d? li?u c?m may ch? Linux.',
        N'Kinh nghi?m qu?n tr? Ubuntu/CentOS, am hi?u Bash Script va Network.',
        N'Tham gia tr?c tr?c ca nh?n ph? c?p h?p d?n, qua t?ng sinh nh?t.',
        N'Linux, Bash Script, Network',
        N'Mid-Level',
        N'Full-time',
        15000000,
        23000000,
        2,
        '2026-07-02',
        'APPROVED',
        50,
        4,
        '2026-05-26 10:30:00',
        NULL,
        '2026-05-25 09:30:00',
        8
    ),
    -- 20. CLOSED
    (
        4,
        4,
        2,
        N'PHP Laravel Web Developer',
        N'B?o tri, phat tri?n m? r?ng cac tinh n?ng m?i cho c?ng thong tin.',
        N'Co kinh nghi?m l?p trinh PHP t?t va t?i thi?u 1 n?m lam v?i Laravel.',
        N'??ng nghi?p than thi?n, s?p tam ly, nhi?u ho?t ??ng n?i b?.',
        N'PHP, Laravel, MySQL',
        N'Junior',
        N'Full-time',
        10000000,
        16000000,
        2,
        '2026-05-15',
        'CLOSED',
        35,
        11,
        '2026-04-11 15:00:00',
        NULL,
        '2026-04-10 16:30:00',
        8
    );
-- =============================================
-- 2. INSERT MAPPING SANG B?NG job_technology
-- (M?i Job co ng?u nhien 2 ??n 3 Tech stacks duy nh?t)
-- =============================================
DECLARE @CurrentRow INT = 1;
DECLARE @TotalInserted INT = (
        SELECT COUNT(*)
        FROM @InsertedJobs
    );
DECLARE @TargetJobId INT;
WHILE @CurrentRow <= @TotalInserted BEGIN
SELECT @TargetJobId = JobId
FROM @InsertedJobs
WHERE RowNumber = @CurrentRow;
-- L?a ch?n k?ch b?n chen Tech ng?u nhien xen k? ?? tranh trung l?p c?p Khoa chinh (job_id, tech_id)
IF @CurrentRow % 4 = 0 BEGIN
INSERT INTO dbo.job_tech_stack(job_id, tech_id)
VALUES (@TargetJobId, 1),
    (@TargetJobId, 2),
    (@TargetJobId, 3);
-- .NET / C# / SQL
END
ELSE IF @CurrentRow % 4 = 1 BEGIN
INSERT INTO dbo.job_tech_stack (job_id, tech_id)
VALUES (@TargetJobId, 4),
    (@TargetJobId, 5);
-- Java / Spring Boot
END
ELSE IF @CurrentRow % 4 = 2 BEGIN
INSERT INTO dbo.job_tech_stack (job_id, tech_id)
VALUES (@TargetJobId, 6),
    (@TargetJobId, 7),
    (@TargetJobId, 8);
-- Python / Django / React
END
ELSE BEGIN
INSERT INTO dbo.job_tech_stack (job_id, tech_id)
VALUES (@TargetJobId, 9),
    (@TargetJobId, 10);
-- NodeJS / Angular
END
SET @CurrentRow = @CurrentRow + 1;
END;
GO
USE ITRecruitmentDB;
GO
SET NOCOUNT ON;
-- Khai bao c?u truc b?ng t?m ?? l?u l?i danh sach JobId t? ??ng sinh ra
DECLARE @NewInsertedJobs TABLE (
        RowNumber INT IDENTITY(1, 1),
        JobId INT
    );
-- =============================================
-- 1. INSERT 10 JOBPOSTS TR?NG THAI APPROVED
-- =============================================
INSERT INTO dbo.job_post (
        company_id,
        position_id,
        company_package_history_id,
        title,
        description,
        requirement,
        benefit,
        skill,
        experience_level,
        working_model,
        salary_min,
        salary_max,
        hiring_quota,
        deadline,
        status,
        priority_score,
        application_count,
        approved_at,
        rejected_reason,
        created_at,
        moderator_id
    ) OUTPUT inserted.job_id INTO @NewInsertedJobs(JobId)
VALUES -- 1: Intern + Fulltime Onsite
    (
        5,
        2,
        2,
        N'Th?c t?p sinh L?p trinh Python (Intern)',
        N'Tham gia h? tr? ??i ng? phat tri?n xay d?ng cac cong c? thu th?p va x? ly s? li?u.',
        N'Sinh vien n?m 3-4 chuyen nganh CNTT, n?m ch?c ki?n th?c OOP c?n b?n.',
        N'H? tr? ph? c?p th?c t?p, co c? h?i len chinh th?c sau 3 thang.',
        N'Python, OOP',
        N'Intern',
        N'Fulltime Onsite',
        3000000,
        6000000,
        5,
        '2026-08-30',
        'APPROVED',
        40,
        2,
        '2026-06-02 10:00:00',
        NULL,
        '2026-06-01 15:30:00',
        8
    ),
    -- 2: Fresher + Hybrid
    (
        5,
        3,
        2,
        N'Fresher ReactJS Developer',
        N'???c ?ao t?o va tham gia tr?c ti?p vao d? an Web App c?a ??i tac Singapore.',
        N'?a co ?? an n?n t?ng HTML/CSS/JS t?t, bi?t c? b?n v? React hooks.',
        N'Review l??ng sau th?i gian th? vi?c, h? tr? thi?t b? lam vi?c.',
        N'ReactJS, JavaScript',
        N'Fresher',
        N'Hybrid',
        8000000,
        12000000,
        3,
        '2026-07-25',
        'APPROVED',
        45,
        7,
        '2026-06-03 09:15:00',
        NULL,
        '2026-06-02 11:00:00',
        8
    ),
    -- 3: Junior + Fulltime Remote
    (
        5,
        11,
        2,
        N'Junior QA Manual Tester',
        N'Th?c hi?n vi?t testcase va th?c thi ki?m th? cac ch?c n?ng h? th?ng th??ng m?i ?i?n t?.',
        N'Co t? 1 n?m kinh nghi?m test web/app, hi?u bi?t v? quy trinh Agile/Scrum.',
        N'Lam vi?c Remote t? do, cung c?p tai kho?n h?c t?p Udemy Business.',
        N'Manual Test, Testcase',
        N'Junior',
        N'Fulltime Remote',
        11000000,
        16000000,
        2,
        '2026-07-15',
        'APPROVED',
        50,
        4,
        '2026-05-30 14:00:00',
        NULL,
        '2026-05-29 14:30:00',
        8
    ),
    -- 4: Middle + Parttime
    (
        5,
        1,
        2,
        N'C# .NET Core Developer (Part-time)',
        N'H? tr? b?o tri, nang c?p m?t s? module c? thu?c h? th?ng qu?n tr? n?i b?.',
        N'T?i thi?u 2 n?m kinh nghi?m lam vi?c v?i .NET MVC / .NET Core, SQL Server.',
        N'Th?i gian lam vi?c linh ho?t, tr? l??ng theo gi? ho?c theo goi task hoan thanh.',
        N'C#, .NET Core',
        N'Middle',
        N'Parttime',
        10000000,
        15000000,
        1,
        '2026-06-28',
        'APPROVED',
        55,
        1,
        '2026-06-01 16:00:00',
        NULL,
        '2026-05-31 16:30:00',
        8
    ),
    -- 5: Senior + Freelance
    (
        5,
        4,
        2,
        N'Fullstack Node/Vue Expert (Freelance)',
        N'Ch?u trach nhi?m ki?n truc l?i ph?n API Gateway va t?i ?u hoa UI/UX ?ng d?ng.',
        N'Tr?c chi?n tren 4 n?m Fullstack, co s?n ph?m th?c t? ch?ng minh n?ng l?c.',
        N'Thu lao d? an c?c k? h?p d?n, lam vi?c ??c l?p khong go bo.',
        N'NodeJS, VueJS',
        N'Senior',
        N'Freelance',
        30000000,
        50000000,
        2,
        '2026-07-10',
        'APPROVED',
        70,
        3,
        '2026-05-28 11:20:00',
        NULL,
        '2026-05-27 13:00:00',
        8
    ),
    -- 6: Lead / Manager + Fulltime Onsite
    (
        5,
        16,
        2,
        N'Project Manager (IT PM)',
        N'Qu?n ly vong ??i d? an, lam vi?c tr?c ti?p v?i khach hang Nh?t B?n ?? ch?t spec.',
        N'T?i thi?u 5 n?m kinh nghi?m ph?n m?m, giao ti?p ti?ng Nh?t t? N2 tr? len.',
        N'Th??ng qu?n ly, goi ch?m soc s?c kh?e VIP cho c? gia ?inh.',
        N'Project Management, Agile',
        N'Lead',
        N'Fulltime Onsite',
        40000000,
        65000000,
        1,
        '2026-07-20',
        'APPROVED',
        90,
        5,
        '2026-05-25 09:00:00',
        NULL,
        '2026-05-24 10:00:00',
        8
    ),
    -- 7: Middle + Fulltime Onsite
    (
        5,
        8,
        2,
        N'DevOps Cloud Engineer',
        N'Tri?n khai h? t?ng h? t?ng vi?n thong s? d?ng Docker, K8s tren n?n t?ng GCP.',
        N'Co kinh nghi?m build CI/CD pipelines, am hi?u sau h? ?i?u hanh Linux.',
        N'L??ng net c?nh tranh, ??nh h??ng phat tri?n ro rang len Architect.',
        N'Docker, K8s, CI/CD',
        N'Middle',
        N'Fulltime Onsite',
        22000000,
        35000000,
        2,
        '2026-08-01',
        'APPROVED',
        75,
        2,
        '2026-06-02 15:30:00',
        NULL,
        '2026-06-02 09:00:00',
        8
    ),
    -- 8: Junior + Hybrid
    (
        5,
        7,
        2,
        N'Flutter Mobile Developer',
        N'Xay d?ng cac module giao di?n ng??i dung cho ?ng d?ng ??t ?? ?n tr?c tuy?n.',
        N'Co tren 1 n?m kinh nghi?m l?p trinh ?ng d?ng di ??ng Flutter/Dart.',
        N'Moi tr??ng cong ngh? hi?n ??i, tu?n lam vi?c 2 ngay remote.',
        N'Flutter, Dart',
        N'Junior',
        N'Hybrid',
        14000000,
        20000000,
        2,
        '2026-07-18',
        'APPROVED',
        60,
        9,
        '2026-06-01 10:00:00',
        NULL,
        '2026-05-31 15:00:00',
        8
    ),
    -- 9: Senior + Fulltime Remote
    (
        5,
        18,
        2,
        N'Senior Data Engineer',
        N'Xay d?ng, b?o tri h? t?ng Data Warehouse, lu?ng x? ly ETL d? li?u tai chinh.',
        N'H?n 3 n?m kinh nghi?m v?i Big Data, am hi?u sau Hadoop, Spark, Kafka.',
        N'Lam vi?c t? xa 100%, tr? c?p chi phi mua s?m gh? cong thai h?c.',
        N'Hadoop, Spark, ETL',
        N'Senior',
        N'Fulltime Remote',
        35000000,
        55000000,
        1,
        '2026-07-30',
        'APPROVED',
        85,
        4,
        '2026-05-20 16:45:00',
        NULL,
        '2026-05-20 09:30:00',
        8
    ),
    -- 10: Fresher + Parttime
    (
        5,
        14,
        2,
        N'Tr? ly Business Analyst (Part-time BA)',
        N'Tham gia v? bi?u ?? UseCase, vi?t tai li?u ??c t? h? th?ng cung Senior BA.',
        N'T? duy logic t?t, bi?t s? d?ng cong c? Figma, Miro ho?c Visio.',
        N'C? h?i h?c h?i quy trinh chu?n ch?nh, phu h?p lam them tich l?y kinh nghi?m.',
        N'BA, UML, Figma',
        N'Fresher',
        N'Parttime',
        5000000,
        8000000,
        2,
        '2026-07-05',
        'APPROVED',
        45,
        12,
        '2026-05-29 11:00:00',
        NULL,
        '2026-05-28 14:00:00',
        8
    );
-- =============================================
-- 2. INSERT MAPPING SANG B?NG job_tech_stack
-- =============================================
DECLARE @CurrentRow INT = 1;
DECLARE @TotalInserted INT = (
        SELECT COUNT(*)
        FROM @NewInsertedJobs
    );
DECLARE @TargetJobId INT;
WHILE @CurrentRow <= @TotalInserted BEGIN
SELECT @TargetJobId = JobId
FROM @NewInsertedJobs
WHERE RowNumber = @CurrentRow;
-- Phan b? tech_id ng?u nhien xoay vong ?? t?o d? li?u ?a d?ng sinh ??ng
IF @CurrentRow % 3 = 0 BEGIN
INSERT INTO dbo.job_tech_stack(job_id, tech_id)
VALUES (@TargetJobId, 1),
    (@TargetJobId, 2);
-- C# / .NET
END
ELSE IF @CurrentRow % 3 = 1 BEGIN
INSERT INTO dbo.job_tech_stack(job_id, tech_id)
VALUES (@TargetJobId, 6),
    (@TargetJobId, 8),
    (@TargetJobId, 9);
-- Python / React / NodeJS
END
ELSE BEGIN
INSERT INTO dbo.job_tech_stack(job_id, tech_id)
VALUES (@TargetJobId, 4),
    (@TargetJobId, 10);
-- Java / Angular
END
SET @CurrentRow = @CurrentRow + 1;
END;
-- Map provinces for the new jobs
DECLARE @NewJobLocations TABLE (RowNumber INT IDENTITY(1, 1), ProvinceName NVARCHAR(100));
INSERT INTO @NewJobLocations (ProvinceName)
VALUES (N'Ha N?i'), (N'H? Chi Minh'), (N'?a N?ng'), (N'Ha N?i'), (N'Ha N?i'),
       (N'H? Chi Minh'), (N'H? Chi Minh'), (N'Ha N?i'), (N'Ha N?i'), (N'Ha N?i');
INSERT INTO dbo.job_post_province (job_id, province_id)
SELECT i.JobId, p.province_id
FROM @NewInsertedJobs i
JOIN @NewJobLocations jl ON i.RowNumber = jl.RowNumber
JOIN dbo.province p ON jl.ProvinceName = p.province_name;
GO -- =============================================
    -- 3. INSERT MAPPING SANG B?NG job_post_province
    -- =============================================
DECLARE @JobLocations TABLE (
        RowNumber INT IDENTITY(1, 1),
        ProvinceName NVARCHAR(100)
    );
INSERT INTO @JobLocations (ProvinceName)
VALUES (N'Ha N?i'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'H? Chi Minh'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'H? Chi Minh'),
    (N'Ha N?i'),
    (N'Ha N?i'),
    (N'H? Chi Minh'),
    (N'H? Chi Minh'),
    (N'?a N?ng'),
    (N'Ha N?i'),
    (N'Ha N?i');
INSERT INTO dbo.job_post_province (job_id, province_id)
SELECT i.JobId,
    p.province_id
FROM @InsertedJobs i
    JOIN @JobLocations jl ON i.RowNumber = jl.RowNumber
    JOIN dbo.province p ON jl.ProvinceName = p.province_name;
GO -- =============================================
    -- 4. TRUY V?N KI?M TRA L?I K?T QU?
    -- =============================================
SELECT j.job_id,
    j.title,
    j.experience_level,
    j.working_model,
    j.status,
    j.deadline,
    (
        SELECT COUNT(*)
        FROM dbo.job_tech_stack jt
        WHERE jt.job_id = j.job_id
    ) AS TechStackCount
FROM dbo.job_post j
WHERE j.company_id = 4
    AND j.status = 'APPROVED'
ORDER BY j.job_id DESC;
GO

