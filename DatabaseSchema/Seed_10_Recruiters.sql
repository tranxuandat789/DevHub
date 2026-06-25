USE ITRecruitmentDB;
GO
SET NOCOUNT ON;

-- =============================================================
-- SEED: 10 RECRUITERS MỚI + HỒ SƠ CÔNG TY + JOB POSTS + REVIEWS
-- user_id  : 23 – 32  (recruiter)
-- job_id   : auto-generated (IDENTITY)
-- =============================================================

-- -------------------------------------------------------------
-- 1. USER ACCOUNTS (10 recruiter)
-- -------------------------------------------------------------
SET IDENTITY_INSERT dbo.user_account ON;
INSERT INTO dbo.user_account (user_id, email, password_hash, user_type, is_active, created_at, last_login) VALUES
(23, 'hr@tpbank.com.vn',        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(24, 'talent@momo.vn',          '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(25, 'hr@topcv.vn',             '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(26, 'recruit@got-it.ai',       '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(27, 'hr@tiki.vn',              '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(28, 'talent@kms-technology.com','$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(29, 'hr@axon-active.com',      '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(30, 'recruit@skymavis.com',    '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(31, 'hr@zalopay.vn',           '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE()),
(32, 'talent@base.vn',          '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja', 'RECRUITER', 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT dbo.user_account OFF;

-- -------------------------------------------------------------
-- 2. RECRUITER PROFILES (hồ sơ công ty đầy đủ, is_verified=1)
-- -------------------------------------------------------------
INSERT INTO dbo.recruiter
    (recruiter_id, full_name, position, phone, company_name, company_address,
     company_description, website, industry, tax_code,
     average_rating, total_reviews, is_verified, profile_completion)
VALUES
(23, N'Nguyễn Minh Tú',    N'HR Manager',          '0901111222', N'TPBank',
     N'Hà Nội',
     N'Ngân hàng TMCP Tiên Phong – tiên phong ứng dụng công nghệ số trong ngân hàng.',
     'https://tpbank.com.vn', N'Ngân hàng & Fintech', '0101999888', 0.00, 0, 1, 90),

(24, N'Trần Thị Lan Anh',  N'Talent Acquisition',  '0912222333', N'MoMo',
     N'Hồ Chí Minh',
     N'Ví điện tử MoMo – nền tảng thanh toán di động hàng đầu Việt Nam với 30M+ người dùng.',
     'https://momo.vn', N'Fintech', '0302111999', 0.00, 0, 1, 90),

(25, N'Lê Văn Hùng',       N'HR Lead',             '0923333444', N'TopCV',
     N'Hà Nội',
     N'Nền tảng tuyển dụng và việc làm hàng đầu Việt Nam, kết nối 5M+ ứng viên với nhà tuyển dụng.',
     'https://topcv.vn', N'HR Tech', '0101777666', 0.00, 0, 1, 90),

(26, N'Phạm Thu Hà',       N'Recruitment Manager', '0934444555', N'Got It',
     N'Hà Nội',
     N'Startup công nghệ giáo dục hàng đầu, kết nối sinh viên với chuyên gia toàn cầu.',
     'https://got-it.ai', N'EdTech & AI', '0103555444', 0.00, 0, 1, 90),

(27, N'Đỗ Hoàng Nam',      N'HR Director',         '0945555666', N'Tiki',
     N'Hồ Chí Minh',
     N'Sàn thương mại điện tử Tiki – "Giao siêu tốc, đủ mọi thứ" với 25M+ khách hàng.',
     'https://tiki.vn', N'E-Commerce', '0312333222', 0.00, 0, 1, 90),

(28, N'Vũ Thị Mai',        N'Talent Manager',      '0956666777', N'KMS Technology',
     N'Hồ Chí Minh',
     N'Công ty phần mềm outsourcing chất lượng cao, chuyên phục vụ thị trường Mỹ và Úc.',
     'https://kms-technology.com', N'Phần mềm', '0302444111', 0.00, 0, 1, 90),

(29, N'Hoàng Quốc Bảo',   N'HR Specialist',       '0967777888', N'Axon Active',
     N'Hà Nội',
     N'Công ty CNTT Thụy Sĩ – Việt Nam chuyên phát triển phần mềm Agile cho khách hàng châu Âu.',
     'https://axon-active.com', N'Phần mềm', '0101666555', 0.00, 0, 1, 90),

(30, N'Bùi Minh Châu',    N'People & Culture',    '0978888999', N'Sky Mavis',
     N'Hồ Chí Minh',
     N'Studio game blockchain nổi tiếng toàn cầu, tác giả của Axie Infinity.',
     'https://skymavis.com', N'Game & Blockchain', '0315111888', 0.00, 0, 1, 90),

(31, N'Nguyễn Thu Phương', N'HR Manager',          '0989999000', N'ZaloPay',
     N'Hồ Chí Minh',
     N'Ví điện tử ZaloPay thuộc hệ sinh thái Zalo – VNG, phục vụ hàng triệu giao dịch mỗi ngày.',
     'https://zalopay.vn', N'Fintech', '0302555888', 0.00, 0, 1, 90),

(32, N'Trần Đức Thắng',   N'HR Lead',             '0910000111', N'Base.vn',
     N'Hà Nội',
     N'Nền tảng quản trị doanh nghiệp toàn diện – phần mềm SaaS B2B hàng đầu Việt Nam.',
     'https://base.vn', N'SaaS & B2B', '0104888777', 0.00, 0, 1, 90);

-- -------------------------------------------------------------
-- 3. JOB POSTS (3 job / công ty, tổng 30 jobs, tất cả APPROVED)
-- -------------------------------------------------------------
DECLARE @Jobs TABLE (RowNum INT IDENTITY(1,1), JobId INT);

INSERT INTO dbo.job_post
    (recruiter_id, position_id, title, description, requirement, benefit,
     skill, experience_level, location, working_model,
     salary_min, salary_max, hiring_quota, deadline,
     status, priority_score, application_count, approved_at, created_at, moderator_id)
OUTPUT inserted.job_id INTO @Jobs(JobId)
VALUES
-- ===== TPBank (23) =====
(23, 1,  N'Senior Java Backend Engineer',
     N'Phát triển core banking system tại TPBank, xây dựng API giao dịch tài chính tốc độ cao.',
     N'4+ năm Java Spring Boot, hiểu biết về Kafka, Redis, tư duy hệ thống tốt.',
     N'Bảo hiểm sức khỏe VPM, thưởng KPI hàng quý, du lịch 2 lần/năm.',
     N'Java, Spring Boot, Kafka', N'Senior', N'Hà Nội', N'Full-time',
     28000000, 45000000, 2, '2026-09-30', 'APPROVED', 85, 5, '2026-06-10 09:00:00', '2026-06-09 10:00:00', 8),

(23, 18, N'Data Engineer – Fintech Analytics',
     N'Xây dựng pipeline dữ liệu phục vụ phân tích rủi ro tín dụng và hành vi khách hàng.',
     N'3+ năm Data Engineering, thành thạo Spark, Airflow, SQL nâng cao.',
     N'Môi trường fintech năng động, cổ phần thưởng, làm việc hybrid.',
     N'Python, Spark, Airflow', N'Mid', N'Hà Nội', N'Hybrid',
     22000000, 35000000, 1, '2026-09-15', 'APPROVED', 75, 3, '2026-06-08 14:00:00', '2026-06-07 15:00:00', 8),

(23, 4,  N'Fullstack Developer – Mobile Banking',
     N'Phát triển ứng dụng Mobile Banking thế hệ mới cho TPBank trên nền tảng React Native.',
     N'2+ năm React Native hoặc Flutter, có kinh nghiệm tích hợp payment API.',
     N'Thiết bị Apple cấp phát, học phí AWS/GCP, review lương 2 lần/năm.',
     N'React Native, TypeScript, REST API', N'Mid', N'Hà Nội', N'Hybrid',
     18000000, 28000000, 2, '2026-09-01', 'APPROVED', 70, 4, '2026-06-07 11:00:00', '2026-06-06 09:00:00', 8),

-- ===== MoMo (24) =====
(24, 3,  N'Senior Frontend Developer – MoMo Super App',
     N'Xây dựng giao diện người dùng cho MoMo Super App phục vụ 30M+ người dùng.',
     N'4+ năm ReactJS/NextJS, thành thạo performance optimization, micro-frontend.',
     N'Cổ phần ESOP, bữa trưa miễn phí, phòng gym tại văn phòng.',
     N'ReactJS, NextJS, TypeScript', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     30000000, 50000000, 2, '2026-09-30', 'APPROVED', 90, 8, '2026-06-10 09:00:00', '2026-06-09 08:00:00', 8),

(24, 19, N'ML Engineer – Credit Scoring',
     N'Xây dựng mô hình chấm điểm tín dụng và phát hiện gian lận giao dịch tại MoMo.',
     N'2+ năm ML/AI, thành thạo Python, scikit-learn, PyTorch hoặc TensorFlow.',
     N'Làm việc cạnh đội ngũ AI hàng đầu Việt Nam, lương thưởng cực kỳ cạnh tranh.',
     N'Python, PyTorch, ML', N'Mid', N'Hồ Chí Minh', N'Full-time',
     25000000, 42000000, 1, '2026-08-31', 'APPROVED', 88, 6, '2026-06-09 10:30:00', '2026-06-08 09:00:00', 8),

(24, 8,  N'DevOps / SRE Engineer',
     N'Vận hành hạ tầng cloud quy mô lớn phục vụ hàng triệu giao dịch mỗi ngày.',
     N'3+ năm DevOps, thành thạo Kubernetes, Terraform, AWS hoặc GCP.',
     N'Chứng chỉ cloud được tài trợ, phụ cấp ăn tối khi overtime, bảo hiểm premium.',
     N'Kubernetes, Terraform, AWS', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     28000000, 45000000, 1, '2026-09-15', 'APPROVED', 82, 4, '2026-06-08 15:00:00', '2026-06-07 10:00:00', 8),

-- ===== TopCV (25) =====
(25, 3,  N'Frontend Developer – React/NextJS',
     N'Phát triển tính năng mới cho nền tảng tuyển dụng TopCV phục vụ 5M+ ứng viên.',
     N'2+ năm ReactJS, NextJS, hiểu biết về SEO và Web Performance.',
     N'Remote 3 ngày/tuần, cấp MacBook, review lương hàng năm.',
     N'ReactJS, NextJS, SEO', N'Mid', N'Hà Nội', N'Hybrid',
     16000000, 26000000, 3, '2026-09-01', 'APPROVED', 68, 7, '2026-06-10 10:00:00', '2026-06-09 09:00:00', 8),

(25, 1,  N'Backend Developer – Python/Django',
     N'Xây dựng API matching engine kết nối ứng viên – nhà tuyển dụng tại TopCV.',
     N'2+ năm Python Django/FastAPI, biết Elasticsearch là lợi thế.',
     N'Môi trường startup năng động, cơ hội học hỏi từ các vấn đề scale lớn.',
     N'Python, Django, Elasticsearch', N'Junior/Mid', N'Hà Nội', N'Remote',
     14000000, 22000000, 2, '2026-09-15', 'APPROVED', 62, 5, '2026-06-09 14:00:00', '2026-06-08 10:00:00', 8),

(25, 14, N'Business Analyst – HR Tech',
     N'Phân tích yêu cầu nghiệp vụ tuyển dụng, thiết kế luồng sản phẩm tại TopCV.',
     N'2+ năm BA trong lĩnh vực HR hoặc SaaS, viết tài liệu đặc tả rõ ràng.',
     N'Làm việc cùng đội sản phẩm giàu kinh nghiệm, môi trường học hỏi cao.',
     N'Business Analysis, UML, Jira', N'Mid', N'Hà Nội', N'Full-time',
     15000000, 24000000, 1, '2026-08-31', 'APPROVED', 60, 3, '2026-06-08 11:00:00', '2026-06-07 08:00:00', 8),

-- ===== Got It (26) =====
(26, 19, N'AI Engineer – NLP/LLM',
     N'Phát triển các ứng dụng AI giáo dục dựa trên LLM, tích hợp GPT-4 vào nền tảng.',
     N'Nền tảng toán/ML vững, có kinh nghiệm fine-tuning LLM hoặc Prompt Engineering.',
     N'Môi trường nghiên cứu cạnh PhD quốc tế, tài liệu kỹ thuật phong phú.',
     N'Python, LLM, NLP', N'Mid/Senior', N'Hà Nội', N'Hybrid',
     30000000, 55000000, 2, '2026-09-30', 'APPROVED', 92, 9, '2026-06-10 09:00:00', '2026-06-09 07:00:00', 8),

(26, 1,  N'Backend Engineer – Go/Microservices',
     N'Xây dựng backend chịu tải cao cho hệ thống matching học sinh – gia sư toàn cầu.',
     N'2+ năm Go hoặc Rust, kinh nghiệm gRPC, message queue (Kafka/RabbitMQ).',
     N'Remote-first, đội ngũ quốc tế, văn hóa học hỏi liên tục.',
     N'Go, gRPC, Kafka', N'Mid', N'Hà Nội', N'Remote',
     22000000, 38000000, 1, '2026-09-15', 'APPROVED', 80, 5, '2026-06-09 14:00:00', '2026-06-08 09:00:00', 8),

(26, 20, N'UI/UX Designer – EdTech Product',
     N'Thiết kế trải nghiệm người dùng cho ứng dụng học tập AI phục vụ học sinh toàn cầu.',
     N'3+ năm UX Design, thành thạo Figma, có danh mục sản phẩm giáo dục là lợi thế.',
     N'Sáng tạo không giới hạn, team nhỏ nhưng impact lớn, lương hấp dẫn.',
     N'Figma, UI/UX, Prototyping', N'Mid', N'Hà Nội', N'Hybrid',
     18000000, 30000000, 1, '2026-08-31', 'APPROVED', 72, 4, '2026-06-08 10:00:00', '2026-06-07 09:00:00', 8),

-- ===== Tiki (27) =====
(27, 1,  N'Senior Backend Engineer – E-Commerce Platform',
     N'Phát triển hệ thống đơn hàng và thanh toán quy mô lớn tại Tiki (25M+ khách hàng).',
     N'4+ năm Java hoặc Go, kinh nghiệm với distributed systems, database sharding.',
     N'ESOP hấp dẫn, bảo hiểm sức khỏe toàn gia đình, teambuilding hàng quý.',
     N'Java, Go, Kafka, Redis', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     35000000, 55000000, 2, '2026-09-30', 'APPROVED', 88, 10, '2026-06-10 09:00:00', '2026-06-09 08:00:00', 8),

(27, 18, N'Data Analyst – E-Commerce Insights',
     N'Phân tích hành vi mua sắm, tối ưu chiến lược giá và khuyến mãi tại Tiki.',
     N'2+ năm Data Analysis, thành thạo SQL, Python, Power BI hoặc Tableau.',
     N'Làm việc cùng data team lớn nhất Việt Nam, lộ trình lên Data Scientist rõ ràng.',
     N'SQL, Python, Power BI', N'Mid', N'Hồ Chí Minh', N'Full-time',
     18000000, 30000000, 2, '2026-09-15', 'APPROVED', 70, 6, '2026-06-09 10:00:00', '2026-06-08 09:00:00', 8),

(27, 5,  N'Mobile Developer – Tiki App (iOS/Android)',
     N'Phát triển tính năng mới cho Tiki App phục vụ hàng chục triệu giao dịch mỗi ngày.',
     N'2+ năm React Native hoặc Flutter, kinh nghiệm tối ưu hiệu năng ứng dụng.',
     N'Cấp iPhone/MacBook, cơm trưa miễn phí, bảo hiểm nhân thọ.',
     N'React Native, Flutter, TypeScript', N'Mid', N'Hồ Chí Minh', N'Full-time',
     20000000, 35000000, 2, '2026-09-01', 'APPROVED', 75, 7, '2026-06-08 14:00:00', '2026-06-07 10:00:00', 8),

-- ===== KMS Technology (28) =====
(28, 1,  N'.NET Core Developer – US Market',
     N'Phát triển phần mềm doanh nghiệp cho khách hàng Mỹ, làm việc theo mô hình Agile.',
     N'3+ năm .NET Core/C#, SQL Server, giao tiếp tiếng Anh tốt.',
     N'Cơ hội onsite Mỹ/Úc, tài trợ chứng chỉ, review lương 2 lần/năm.',
     N'C#, .NET Core, SQL Server', N'Mid/Senior', N'Hồ Chí Minh', N'Full-time',
     22000000, 40000000, 3, '2026-09-30', 'APPROVED', 78, 8, '2026-06-10 09:00:00', '2026-06-09 07:00:00', 8),

(28, 11, N'QA Automation Engineer',
     N'Xây dựng framework kiểm thử tự động cho các dự án phần mềm outsourcing tại KMS.',
     N'2+ năm Automation Testing, thành thạo Selenium/Playwright, Python hoặc Java.',
     N'Môi trường chuyên nghiệp, lộ trình kỹ thuật rõ ràng, không OT.',
     N'Selenium, Playwright, Python', N'Mid', N'Hồ Chí Minh', N'Hybrid',
     16000000, 26000000, 2, '2026-09-15', 'APPROVED', 65, 4, '2026-06-09 14:00:00', '2026-06-08 10:00:00', 8),

(28, 8,  N'DevOps Engineer – AWS/Azure',
     N'Quản lý hạ tầng CI/CD cho các dự án phần mềm quy mô lớn phục vụ khách hàng nước ngoài.',
     N'3+ năm DevOps, kinh nghiệm AWS hoặc Azure, có chứng chỉ cloud là lợi thế.',
     N'Chứng chỉ được tài trợ 100%, bảo hiểm sức khỏe, teambuilding quốc tế.',
     N'AWS, Azure, Docker, CI/CD', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     28000000, 45000000, 1, '2026-09-01', 'APPROVED', 80, 3, '2026-06-08 10:00:00', '2026-06-07 08:00:00', 8),

-- ===== Axon Active (29) =====
(29, 4,  N'Fullstack Developer – Java/Angular',
     N'Phát triển phần mềm cho khách hàng châu Âu theo quy trình Agile tại Axon Active.',
     N'3+ năm Java Spring Boot, 1+ năm Angular, giao tiếp tiếng Anh tốt.',
     N'Onsite Thụy Sĩ 3–6 tháng/năm, lương ngoại tệ, môi trường đa văn hóa.',
     N'Java, Angular, TypeScript', N'Mid/Senior', N'Hà Nội', N'Full-time',
     24000000, 42000000, 2, '2026-09-30', 'APPROVED', 85, 6, '2026-06-10 09:00:00', '2026-06-09 08:00:00', 8),

(29, 3,  N'Frontend Developer – ReactJS',
     N'Xây dựng giao diện SaaS cho khách hàng doanh nghiệp tại Thụy Sĩ và Đức.',
     N'2+ năm ReactJS, TypeScript, kinh nghiệm tích hợp RESTful API.',
     N'Cơ hội làm việc với khách hàng trực tiếp châu Âu, phụ cấp ngôn ngữ.',
     N'ReactJS, TypeScript, CSS', N'Mid', N'Hà Nội', N'Hybrid',
     18000000, 30000000, 2, '2026-09-15', 'APPROVED', 72, 5, '2026-06-09 13:00:00', '2026-06-08 09:00:00', 8),

(29, 11, N'QA Engineer – Manual & Automation',
     N'Đảm bảo chất lượng phần mềm cho các dự án outsourcing châu Âu của Axon Active.',
     N'2+ năm QA, kinh nghiệm viết test case và automation testing cơ bản.',
     N'Làm việc theo Agile/Scrum, đồng nghiệp thân thiện, không OT vô lý.',
     N'Manual Testing, Selenium, Jira', N'Junior/Mid', N'Hà Nội', N'Full-time',
     14000000, 22000000, 2, '2026-08-31', 'APPROVED', 60, 3, '2026-06-08 14:00:00', '2026-06-07 10:00:00', 8),

-- ===== Sky Mavis (30) =====
(30, 1,  N'Senior Backend Engineer – Blockchain',
     N'Phát triển smart contract và backend cho hệ sinh thái Axie Infinity, Sky Mavis.',
     N'4+ năm backend (Go/Rust/Java), kinh nghiệm blockchain/Web3 là bắt buộc.',
     N'Token thưởng, môi trường game quốc tế, văn phòng đẳng cấp.',
     N'Go, Rust, Blockchain, Web3', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     50000000, 80000000, 2, '2026-09-30', 'APPROVED', 95, 12, '2026-06-10 09:00:00', '2026-06-09 07:00:00', 8),

(30, 5,  N'Mobile Game Developer – Unity',
     N'Phát triển game mobile đa nền tảng phục vụ hàng triệu game thủ toàn cầu.',
     N'2+ năm Unity/C#, kinh nghiệm tối ưu hiệu năng game mobile.',
     N'Làm việc cùng đội ngũ game developer tài năng nhất Đông Nam Á.',
     N'Unity, C#, Game Dev', N'Mid', N'Hồ Chí Minh', N'Full-time',
     30000000, 50000000, 2, '2026-09-15', 'APPROVED', 90, 10, '2026-06-09 14:00:00', '2026-06-08 09:00:00', 8),

(30, 20, N'UI/UX Designer – Game & NFT Marketplace',
     N'Thiết kế giao diện game và marketplace NFT cho hệ sinh thái Sky Mavis.',
     N'3+ năm UX Design, có danh mục thiết kế game/Web3 là lợi thế lớn.',
     N'Token ESOP, làm việc với sản phẩm có impact toàn cầu, visa support.',
     N'Figma, UI/UX, Web3 Design', N'Mid/Senior', N'Hồ Chí Minh', N'Hybrid',
     25000000, 45000000, 1, '2026-09-01', 'APPROVED', 85, 7, '2026-06-08 10:00:00', '2026-06-07 08:00:00', 8),

-- ===== ZaloPay (31) =====
(31, 1,  N'Senior Backend Developer – Payment Core',
     N'Phát triển core payment engine xử lý hàng triệu giao dịch/ngày tại ZaloPay.',
     N'4+ năm Java Spring Boot hoặc Go, kinh nghiệm fintech/payment là bắt buộc.',
     N'Thu nhập top đầu thị trường, bảo hiểm Bảo Việt premium, ESOP.',
     N'Java, Go, Kafka, Redis', N'Senior', N'Hồ Chí Minh', N'Full-time',
     40000000, 65000000, 2, '2026-09-30', 'APPROVED', 92, 11, '2026-06-10 09:00:00', '2026-06-09 07:00:00', 8),

(31, 8,  N'DevOps / Cloud Engineer – GCP',
     N'Xây dựng và vận hành hạ tầng GCP cho hệ thống payment quy mô lớn ZaloPay.',
     N'3+ năm DevOps, thành thạo GCP, Kubernetes, Terraform, có chứng chỉ GCP.',
     N'Chứng chỉ GCP tài trợ, bảo hiểm premium, văn phòng hiện đại trung tâm HCM.',
     N'GCP, Kubernetes, Terraform', N'Senior', N'Hồ Chí Minh', N'Hybrid',
     35000000, 55000000, 1, '2026-09-15', 'APPROVED', 85, 6, '2026-06-09 14:00:00', '2026-06-08 09:00:00', 8),

(31, 19, N'Data Scientist – Fraud Detection',
     N'Xây dựng mô hình phát hiện gian lận giao dịch theo thời gian thực tại ZaloPay.',
     N'2+ năm Data Science, thành thạo Python, ML, kinh nghiệm fintech là lợi thế.',
     N'Môi trường dữ liệu phong phú, đội ngũ AI/ML hàng đầu, lương rất cạnh tranh.',
     N'Python, ML, Data Science', N'Mid/Senior', N'Hồ Chí Minh', N'Full-time',
     28000000, 48000000, 1, '2026-09-01', 'APPROVED', 88, 8, '2026-06-08 10:00:00', '2026-06-07 08:00:00', 8),

-- ===== Base.vn (32) =====
(32, 3,  N'Frontend Developer – SaaS Platform',
     N'Phát triển giao diện phần mềm quản trị doanh nghiệp Base.vn phục vụ 8000+ doanh nghiệp.',
     N'2+ năm ReactJS hoặc VueJS, kinh nghiệm xây dựng dashboard phức tạp.',
     N'Sản phẩm thuần Việt impact lớn, remote 2 ngày/tuần, review lương định kỳ.',
     N'ReactJS, VueJS, TypeScript', N'Mid', N'Hà Nội', N'Hybrid',
     15000000, 25000000, 3, '2026-09-30', 'APPROVED', 65, 5, '2026-06-10 09:00:00', '2026-06-09 08:00:00', 8),

(32, 1,  N'Backend Developer – Node.js/NestJS',
     N'Phát triển API cho nền tảng SaaS B2B quản trị nhân sự, kế toán, CRM tại Base.vn.',
     N'2+ năm Node.js NestJS, PostgreSQL, kinh nghiệm xây dựng multi-tenant SaaS.',
     N'Làm việc với sản phẩm thực sự phục vụ doanh nghiệp VN, đội ngũ trẻ năng động.',
     N'Node.js, NestJS, PostgreSQL', N'Junior/Mid', N'Hà Nội', N'Full-time',
     14000000, 22000000, 2, '2026-09-15', 'APPROVED', 60, 4, '2026-06-09 10:00:00', '2026-06-08 09:00:00', 8),

(32, 14, N'Business Analyst – ERP/HRM',
     N'Phân tích yêu cầu nghiệp vụ ERP, HRM, CRM cho các khách hàng doanh nghiệp lớn của Base.',
     N'2+ năm BA, có kinh nghiệm ERP hoặc HRM, giao tiếp tốt với khách hàng doanh nghiệp.',
     N'Impact rõ ràng, môi trường chuyên nghiệp, không OT, học hỏi liên tục.',
     N'Business Analysis, ERP, SQL', N'Mid', N'Hà Nội', N'Full-time',
     13000000, 20000000, 2, '2026-08-31', 'APPROVED', 58, 3, '2026-06-08 14:00:00', '2026-06-07 10:00:00', 8);

-- -------------------------------------------------------------
-- 4. JOB TECH STACK (gán tech cho 30 jobs theo recruiter_id)
-- Mỗi nhóm 3 job của 1 công ty nhận tech phù hợp ngành
-- -------------------------------------------------------------
DECLARE @RowNum INT = 1;
DECLARE @TotalJobs INT = (SELECT COUNT(*) FROM @Jobs);
DECLARE @JobId INT;
DECLARE @RecId INT;

WHILE @RowNum <= @TotalJobs
BEGIN
    SELECT @JobId = JobId FROM @Jobs WHERE RowNum = @RowNum;

    -- Nhóm theo RowNum để biết thuộc công ty nào (3 job/công ty)
    DECLARE @GroupIdx INT = ((@RowNum - 1) / 3) + 1; -- 1..10 tương ứng 10 công ty
    DECLARE @JobInGroup INT = ((@RowNum - 1) % 3) + 1; -- 1..3 trong mỗi nhóm

    IF @GroupIdx = 1 -- TPBank
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 1), (@JobId, 2), (@JobId, 13);   -- Java, Spring Boot, Docker
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 18), (@JobId, 30); -- Python, PostgreSQL, SQL
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 12), (@JobId, 10), (@JobId, 6);                    -- React Native, TypeScript, Node.js
    END
    ELSE IF @GroupIdx = 2 -- MoMo
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 10);                    -- ReactJS, TypeScript
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 19);               -- Python, MySQL
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 14), (@JobId, 15), (@JobId, 13);                   -- Kubernetes, AWS, Docker
    END
    ELSE IF @GroupIdx = 3 -- TopCV
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 10), (@JobId, 6);       -- ReactJS, TypeScript, Node.js
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 18), (@JobId, 20); -- Python, PostgreSQL, MongoDB
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 32), (@JobId, 30);                                  -- Business Analyst, SQL
    END
    ELSE IF @GroupIdx = 4 -- Got It
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 20);                    -- Python, MongoDB
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 6), (@JobId, 13);               -- Node.js, Docker
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 10);                                   -- ReactJS, TypeScript
    END
    ELSE IF @GroupIdx = 5 -- Tiki
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 1), (@JobId, 13), (@JobId, 20);      -- Java, Docker, MongoDB
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 31), (@JobId, 30); -- Python, Power BI, SQL
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 12), (@JobId, 11), (@JobId, 10);                   -- React Native, Flutter, TypeScript
    END
    ELSE IF @GroupIdx = 6 -- KMS Technology
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 3), (@JobId, 4), (@JobId, 17);       -- .NET, C#, SQL Server
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 29), (@JobId, 5), (@JobId, 28); -- Selenium, Python, Automation Testing
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 15), (@JobId, 13), (@JobId, 14);                   -- AWS, Docker, Kubernetes
    END
    ELSE IF @GroupIdx = 7 -- Axon Active
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 1), (@JobId, 9), (@JobId, 10);       -- Java, Angular, TypeScript
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 10), (@JobId, 6);  -- ReactJS, TypeScript, Node.js
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 27), (@JobId, 29);                                  -- Manual Testing, Selenium
    END
    ELSE IF @GroupIdx = 8 -- Sky Mavis
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 6), (@JobId, 13), (@JobId, 15);      -- Node.js, Docker, AWS
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 4), (@JobId, 7);                -- C#, ReactJS
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 10);                                   -- ReactJS, TypeScript
    END
    ELSE IF @GroupIdx = 9 -- ZaloPay
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 1), (@JobId, 2), (@JobId, 20);       -- Java, Spring Boot, MongoDB
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 14), (@JobId, 15), (@JobId, 13);-- Kubernetes, AWS, Docker
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 5), (@JobId, 30);                                   -- Python, SQL
    END
    ELSE -- Base.vn (GroupIdx = 10)
    BEGIN
        IF @JobInGroup = 1 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 7), (@JobId, 8), (@JobId, 10);       -- ReactJS, Vue.js, TypeScript
        ELSE IF @JobInGroup = 2 INSERT INTO dbo.job_tech_stack VALUES (@JobId, 6), (@JobId, 18), (@JobId, 10); -- Node.js, PostgreSQL, TypeScript
        ELSE INSERT INTO dbo.job_tech_stack VALUES (@JobId, 32), (@JobId, 30), (@JobId, 17);                   -- Business Analyst, SQL, SQL Server
    END

    SET @RowNum = @RowNum + 1;
END;

-- -------------------------------------------------------------
-- 5. REVIEWS (2–3 reviews / công ty, rating 3–5 sao, APPROVED)
-- Candidate ids dùng: 1,2,3,9,10,11,12,13,14,15,16,17
-- Unique constraint: (candidate_id, recruiter_id) không trùng
-- -------------------------------------------------------------
INSERT INTO dbo.review_recruiter
    (candidate_id, recruiter_id, rating, pros, cons, description,
     is_anonymous, status, moderator_id, moderated_at, created_at, updated_at)
VALUES
-- TPBank (23)
(1,  23, 5, N'Văn hóa học hỏi cao, đồng nghiệp xuất sắc', N'Deadline đôi khi gấp', N'Môi trường làm việc chuyên nghiệp, sếp hỗ trợ tốt.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-10,GETDATE()), GETDATE()),
(2,  23, 4, N'Lương cạnh tranh, văn phòng hiện đại', N'Cần cải thiện quy trình onboarding', N'Đội ngũ kỹ thuật giỏi, nhiều cơ hội phát triển bản thân.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-8,GETDATE()), GETDATE()),
(3,  23, 4, N'Phúc lợi tốt, công việc thú vị', N'Áp lực fintech khá cao', N'Tốt cho người muốn học công nghệ ngân hàng.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-5,GETDATE()), GETDATE()),

-- MoMo (24)
(9,  24, 5, N'Sản phẩm tác động hàng triệu người, team tài năng', N'Work-life balance đôi khi chưa tốt', N'Công ty startup có văn hóa năng động, học hỏi rất nhiều.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-12,GETDATE()), GETDATE()),
(10, 24, 5, N'ESOP hấp dẫn, đồng nghiệp xuất sắc', N'Cần adapt nhanh với tốc độ thay đổi', N'MoMo có team engineering rất chất lượng.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-7,GETDATE()), GETDATE()),

-- TopCV (25)
(11, 25, 4, N'Làm sản phẩm HR ý nghĩa, team thân thiện', N'Lương mid-range so với thị trường', N'Phù hợp nếu bạn muốn trải nghiệm startup HR Tech.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-15,GETDATE()), GETDATE()),
(12, 25, 3, N'Môi trường học hỏi, đồng nghiệp nice', N'Quy trình review lương còn chậm', N'Startup tốt nhưng cần cải thiện comp & benefit.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-9,GETDATE()), GETDATE()),

-- Got It (26)
(13, 26, 5, N'Làm việc với AI/LLM cutting-edge, đội ngũ PhD', N'Interview khó, bar cao', N'Tốt nhất cho người yêu AI và muốn nghiên cứu sâu.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-14,GETDATE()), GETDATE()),
(14, 26, 4, N'Remote linh hoạt, team quốc tế', N'Deadline khắt khe', N'Got It có culture R&D rất tốt cho kỹ sư senior.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-6,GETDATE()), GETDATE()),

-- Tiki (27)
(15, 27, 4, N'Scale lớn, bài toán kỹ thuật thú vị', N'OT nhiều trong mùa sale lớn', N'Làm việc với traffic khổng lồ rất tốt cho portfolio.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-11,GETDATE()), GETDATE()),
(16, 27, 4, N'ESOP, bảo hiểm toàn gia đình', N'Áp lực KPI cao', N'Phúc lợi tốt, môi trường cạnh tranh.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-4,GETDATE()), GETDATE()),
(17, 27, 5, N'Bài toán data phong phú, sản phẩm thực tế', N'Cần kinh nghiệm distributed systems', N'Tiki là môi trường lý tưởng cho data engineer muốn scale.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-3,GETDATE()), GETDATE()),

-- KMS Technology (28)
(1,  28, 4, N'Cơ hội onsite nước ngoài, môi trường chuyên nghiệp', N'Thời gian cố định theo giờ Mỹ', N'KMS có process tốt và khách hàng uy tín.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-13,GETDATE()), GETDATE()),
(2,  28, 3, N'Không OT, công việc ổn định', N'Tech stack đôi khi cũ', N'Phù hợp người cần ổn định hơn là startup fast-pace.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-8,GETDATE()), GETDATE()),

-- Axon Active (29)
(3,  29, 5, N'Onsite Thụy Sĩ, đội ngũ châu Âu chuyên nghiệp', N'Cần tiếng Anh thật tốt', N'Axon Active cho trải nghiệm làm việc quốc tế thực sự.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-16,GETDATE()), GETDATE()),
(9,  29, 4, N'Agile thực sự, không chỉ lý thuyết', N'Khách hàng remote đôi khi khó align', N'Môi trường outsourcing chất lượng cao ít gặp ở VN.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-7,GETDATE()), GETDATE()),

-- Sky Mavis (30)
(10, 30, 5, N'Sản phẩm tác động toàn cầu, token thưởng', N'Crypto market volatile ảnh hưởng mood', N'Sky Mavis cho cảm giác làm việc với sản phẩm thế giới thực.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-20,GETDATE()), GETDATE()),
(11, 30, 5, N'Team game developer đỉnh nhất Đông Nam Á', N'Không phù hợp người không thích game/Web3', N'Đây là nơi tốt nhất để học blockchain engineering tại VN.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-12,GETDATE()), GETDATE()),

-- ZaloPay (31)
(12, 31, 5, N'Thu nhập top đầu thị trường, fintech scale lớn', N'Áp lực hệ thống payment cao', N'ZaloPay trả rất tốt và bài toán kỹ thuật cực kỳ thú vị.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-9,GETDATE()), GETDATE()),
(13, 31, 4, N'Bảo hiểm premium, văn phòng trung tâm HCM', N'OT trong các đợt triển khai feature lớn', N'Phúc lợi rất tốt, nên apply nếu bạn giỏi backend.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-5,GETDATE()), GETDATE()),

-- Base.vn (32)
(14, 32, 4, N'Sản phẩm thuần Việt ý nghĩa, team SaaS chuyên nghiệp', N'Lương chưa bằng fintech/game', N'Base.vn tốt cho bạn muốn xây SaaS B2B từ đầu đến cuối.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-11,GETDATE()), GETDATE()),
(15, 32, 3, N'Không OT, môi trường cân bằng', N'Tốc độ phát triển hơi chậm so với startup khác', N'Phù hợp nếu bạn ưu tiên work-life balance.', 1, 'APPROVED', 8, GETDATE(), DATEADD(day,-6,GETDATE()), GETDATE()),

-- Thêm 1 review cho một số công ty để tạo sự đa dạng số lượng
(16, 23, 5, N'Core banking thú vị, đồng nghiệp senior rất giỏi', N'Onboarding dài', N'TPBank đầu tư mạnh vào công nghệ, phù hợp backend senior.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-2,GETDATE()), GETDATE()),
(17, 24, 4, N'Văn hóa open, nhiều sự kiện nội bộ', N'Yêu cầu cao, bar tuyển dụng khó', N'MoMo tốt cho người muốn làm ở công ty product lớn.', 0, 'APPROVED', 8, GETDATE(), DATEADD(day,-1,GETDATE()), GETDATE());

-- -------------------------------------------------------------
-- 6. CẬP NHẬT average_rating và total_reviews trên bảng recruiter
-- -------------------------------------------------------------
UPDATE r
SET
    r.average_rating = CAST(agg.avg_rating AS DECIMAL(3,2)),
    r.total_reviews  = agg.cnt
FROM dbo.recruiter r
INNER JOIN (
    SELECT recruiter_id,
           AVG(CAST(rating AS FLOAT)) AS avg_rating,
           COUNT(*) AS cnt
    FROM dbo.review_recruiter
    WHERE status = 'APPROVED'
    GROUP BY recruiter_id
) agg ON r.recruiter_id = agg.recruiter_id
WHERE r.recruiter_id BETWEEN 23 AND 32;

-- -------------------------------------------------------------
-- Kiểm tra nhanh kết quả
-- -------------------------------------------------------------
SELECT r.recruiter_id, r.company_name, r.average_rating, r.total_reviews,
       (SELECT COUNT(*) FROM dbo.job_post j WHERE j.recruiter_id = r.recruiter_id AND j.status = 'APPROVED') AS approved_jobs
FROM dbo.recruiter r
WHERE r.recruiter_id BETWEEN 23 AND 32
ORDER BY r.recruiter_id;

PRINT '=================================================';
PRINT 'SUCCESS: 10 recruiters + 30 jobs + reviews seeded!';
PRINT '=================================================';
GO
