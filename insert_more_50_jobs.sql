USE DevHub;
GO
SET NOCOUNT ON;

-- =====================================================
-- 1. XÓA CÁC CÔNG TY VÀ JOB MOCK THÊM NÀY NẾU CÓ ĐỂ CHẠY LẠI KHÔNG LỖI
-- =====================================================
PRINT N'Đang dọn dẹp dữ liệu mock bổ sung cũ...';
DELETE FROM dbo.job_post_province WHERE job_id IN (SELECT job_id FROM dbo.job_post WHERE company_id BETWEEN 126 AND 150);
DELETE FROM dbo.job_tech_stack WHERE job_id IN (SELECT job_id FROM dbo.job_post WHERE company_id BETWEEN 126 AND 150);
DELETE FROM dbo.job_post WHERE company_id BETWEEN 126 AND 150;
DELETE FROM dbo.company WHERE company_id BETWEEN 126 AND 150;

-- =====================================================
-- 2. THÊM 25 CÔNG TY CÔNG NGHỆ MỚI KHÁC NHAU (ID 126 ĐẾN 150)
-- =====================================================
PRINT N'Đang thêm 25 công ty mới bổ sung...';
SET IDENTITY_INSERT dbo.company ON;
INSERT INTO dbo.company (company_id, company_name, company_address, company_description, website, industry, tax_code, is_verified, profile_completion, status)
VALUES
(126, N'Công ty Cổ phần VNP Group', N'Hà Nội', N'VNP Group là đơn vị đi đầu trong lĩnh vực thương mại điện tử và truyền thông.', 'https://vnpgroup.vn', N'Internet & Truyền thông', '0101230126', 1, 80, 'APPROVED'),
(127, N'Công ty Cổ phần Công nghệ Teko Việt Nam', N'Hà Nội', N'Teko chuyên cung cấp các giải pháp O2O và ERP cho doanh nghiệp bán lẻ.', 'https://teko.vn', N'Giải pháp công nghệ', '0101230127', 1, 85, 'APPROVED'),
(128, N'Công ty TNHH FPT IS', N'Hà Nội', N'FPT Information System là đơn vị thành viên chuyên tích hợp hệ thống của tập đoàn FPT.', 'https://fis.com.vn', N'Tích hợp hệ thống', '0101230128', 1, 90, 'APPROVED'),
(129, N'Công ty TNHH CMC TS', N'Hà Nội', N'CMC Technology & Solution là doanh nghiệp cung cấp giải pháp chuyển đổi số hàng đầu.', 'https://cmcts.com.vn', N'Chuyển đổi số', '0101230129', 1, 80, 'APPROVED'),
(130, N'Công ty Cổ phần Sendo', N'Hồ Chí Minh', N'Sendo sở hữu siêu ứng dụng mua sắm Sen Đỏ phổ biến tại Việt Nam.', 'https://sendo.vn', N'Thương mại điện tử', '0101230130', 1, 85, 'APPROVED'),
(131, N'Công ty Cổ phần Giải pháp Thanh toán Việt Nam (VNPAY)', N'Hà Nội', N'VNPAY cung cấp dịch vụ cổng thanh toán điện tử hàng đầu tại Việt Nam.', 'https://vnpay.vn', N'Fintech', '0101230131', 1, 90, 'APPROVED'),
(132, N'Công ty Cổ phần Viettel IDC', N'Hà Nội', N'Viettel IDC dẫn đầu về dịch vụ trung tâm dữ liệu và điện toán đám mây.', 'https://viettelidc.com.vn', N'Cloud & Data Center', '0101230132', 1, 85, 'APPROVED'),
(133, N'Công ty TNHH VNPT-IT', N'Hà Nội', N'VNPT-IT chịu trách nhiệm phát triển các sản phẩm CNTT trọng điểm của tập đoàn VNPT.', 'https://vnpt.com.vn', N'Sản phẩm công nghệ', '0101230133', 1, 80, 'APPROVED'),
(134, N'Công ty Cổ phần Mobifone IT', N'Hà Nội', N'Mobifone IT chuyên trách nghiên cứu phát triển phần mềm viễn thông Mobifone.', 'https://mobifone.vn', N'Viễn thông & CNTT', '0101230134', 1, 80, 'APPROVED'),
(135, N'Công ty Cổ phần Appota', N'Hà Nội', N'Appota đi đầu trong các giải pháp giải trí số và ví điện tử AppotaPay.', 'https://appota.com', N'Giải trí số & Fintech', '0101230135', 1, 80, 'APPROVED'),
(136, N'Công ty Cổ phần Gotadi', N'Hồ Chí Minh', N'Gotadi cung cấp hệ thống tìm kiếm vé máy bay và phòng khách sạn trực tuyến.', 'https://gotadi.com', N'Du lịch trực tuyến (OTA)', '0101230136', 1, 80, 'APPROVED'),
(137, N'Công ty TNHH ELSA Speak Việt Nam', N'Hồ Chí Minh', N'ELSA Speak phát triển ứng dụng học tiếng Anh giao tiếp thông qua công nghệ trí tuệ nhân tạo (AI).', 'https://elsaspeak.vn', N'Edtech', '0101230137', 1, 85, 'APPROVED'),
(138, N'Công ty Cổ phần Gapo', N'Hà Nội', N'Gapo sở hữu mạng xã hội nội bộ doanh nghiệp GapoWork và MXH Gapo.', 'https://gapowork.vn', N'Mạng xã hội', '0101230138', 1, 80, 'APPROVED'),
(139, N'Công ty Cổ phần Ahamove', N'Hồ Chí Minh', N'Ahamove là ứng dụng giao hàng siêu tốc trong nội đô hàng đầu.', 'https://ahamove.com', N'Logistics & Delivery', '0101230139', 1, 85, 'APPROVED'),
(140, N'Công ty Cổ phần Be Group', N'Hồ Chí Minh', N'Be Group sở hữu ứng dụng gọi xe đa dịch vụ thuần Việt "be".', 'https://be.com.vn', N'Gọi xe & Giao nhận', '0101230140', 1, 85, 'APPROVED'),
(141, N'Công ty Cổ phần Trusting Social', N'Hà Nội', N'Trusting Social cung cấp giải pháp chấm điểm tín dụng dựa trên Big Data và AI.', 'https://trustingsocial.com', N'Fintech & AI', '0101230141', 1, 90, 'APPROVED'),
(142, N'Công ty Cổ phần Garena Việt Nam', N'Hồ Chí Minh', N'Garena Việt Nam phát hành các tựa game Esport bom tấn hàng đầu.', 'https://garena.vn', N'Game & Esports', '0101230142', 1, 80, 'APPROVED'),
(143, N'Công ty Cổ phần Vexere', N'Hồ Chí Minh', N'Vexere sở hữu hệ thống đặt vé xe khách lớn nhất Việt Nam.', 'https://vexere.com', N'Du lịch trực tuyến', '0101230143', 1, 80, 'APPROVED'),
(144, N'Công ty Cổ phần Luxstay', N'Hà Nội', N'Luxstay kết nối homestay nghỉ dưỡng cao cấp tại Việt Nam.', 'https://luxstay.com', N'Du lịch trực tuyến', '0101230144', 1, 80, 'APPROVED'),
(145, N'Công ty Cổ phần JupViec', N'Hà Nội', N'JupViec kết nối người giúp việc và khách hàng qua ứng dụng công nghệ.', 'https://jupviec.vn', N'Dịch vụ gia đình', '0101230145', 1, 80, 'APPROVED'),
(146, N'Công ty Cổ phần Edumall', N'Hà Nội', N'Edumall là siêu thị khóa học trực tuyến lớn nhất Đông Nam Á.', 'https://edumall.vn', N'Edtech', '0101230146', 1, 80, 'APPROVED'),
(147, N'Công ty Cổ phần Topica Edtech Group', N'Hà Nội', N'Topica dẫn đầu giải pháp đào tạo cử nhân và tiếng Anh trực tuyến.', 'https://topica.edu.vn', N'Edtech', '0101230147', 1, 80, 'APPROVED'),
(148, N'Công ty Cổ phần Waka', N'Hà Nội', N'Waka là nền tảng đọc sách và truyện tranh trực tuyến hàng đầu.', 'https://waka.vn', N'Nội dung số', '0101230148', 1, 80, 'APPROVED'),
(149, N'Công ty Cổ phần VinBigData', N'Hà Nội', N'VinBigData phát triển các giải pháp AI và xử lý dữ liệu lớn cho tập đoàn Vingroup.', 'https://vinbigdata.com', N'Dữ liệu lớn & AI', '0101230149', 1, 90, 'APPROVED'),
(150, N'Công ty TNHH VinaGame (VNG)', N'Hồ Chí Minh', N'VNG là kỳ lân công nghệ đầu tiên của Việt Nam với mảng Game, Zalo.', 'https://vng.com.vn', N'Công nghệ giải trí', '0101230150', 1, 90, 'APPROVED');
SET IDENTITY_INSERT dbo.company OFF;

-- =====================================================
-- 3. THÊM 50 JOBS (2 JOB CHO MỖI CÔNG TY MỚI)
-- =====================================================
PRINT N'Đang thêm 50 tin tuyển dụng bổ sung mới...';

DECLARE @InsertedJobs TABLE (
    RowNumber INT IDENTITY(1, 1),
    JobId INT
);

INSERT INTO dbo.job_post (
    company_id, position_id, title, description, requirement, benefit, skill,
    experience_level, working_model, salary_min, salary_max, hiring_quota,
    deadline, status, created_at
)
OUTPUT inserted.job_id INTO @InsertedJobs(JobId)
VALUES
-- VNP Group (126)
(126, 1, N'Java Web Developer', N'Phát triển ứng dụng thương mại điện tử cốt lõi.', N'Java Core, Spring Boot, MySQL.', N'Lương hấp dẫn, đóng bảo hiểm đầy đủ.', N'Java, MySQL', N'Mid-Level', N'Full-time', 16000000, 26000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(126, 3, N'Frontend VueJS Developer', N'Phát triển giao diện hệ thống tin tức Vatgia.', N'VueJS, HTML/CSS, JavaScript.', N'Không OT, môi trường trẻ trung.', N'Vue.js, HTML, CSS', N'Junior', N'Full-time', 12000000, 18000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Teko (127)
(127, 1, N'Senior Python Backend Developer', N'Phát triển hệ thống phân phối hàng hóa ERP.', N'Python, Flask, PostgreSQL, Docker.', N'Cấp Macbook Pro M3, du lịch nước ngoài hàng năm.', N'Python, PostgreSQL, Docker', N'Senior', N'Full-time', 30000000, 50000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(127, 3, N'ReactJS Web Developer (Core)', N'Tối ưu giao diện thanh toán ví điện tử.', N'ReactJS, TypeScript, Tailwind CSS.', N'Review lương 2 lần/năm, thưởng quý tốt.', N'ReactJS, TypeScript, CSS', N'Mid-Level', N'Hybrid', 18000000, 30000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- FPT IS (128)
(128, 1, N'Senior .NET Core Developer', N'Phát triển các giải pháp ERP và Chính phủ điện tử.', N'C#, ASP.NET Core, SQL Server, 4 năm kinh nghiệm.', N'Bảo hiểm FPT Care cho bản thân và gia đình.', N'C#, .NET, SQL Server', N'Senior', N'Full-time', 25000000, 45000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(128, 11, N'Automation Test Engineer (Java)', N'Viết kịch bản kiểm thử tự động cho hệ thống ngân hàng.', N'Java, Selenium, TestNG, CI/CD.', N'Được đào tạo nâng cao năng lực, hỗ trợ chứng chỉ.', N'Automation, Selenium, Java', N'Mid-Level', N'Full-time', 18000000, 28000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- CMC TS (129)
(129, 8, N'DevOps Engineer (Cloud/K8s)', N'Hỗ trợ khách hàng chuyển đổi hạ tầng lên Azure/AWS.', N'Docker, Kubernetes, Jenkins, Cloud Certified.', N'Laptop cấu hình khủng, văn phòng sang chảnh.', N'DevOps, Docker, Azure', N'Mid-Level', N'Full-time', 20000000, 35000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(129, 14, N'Business Analyst (Agile)', N'Phân tích yêu cầu và thiết kế giải pháp phần mềm doanh nghiệp.', N'Kinh nghiệm viết SRS, mô tả tính năng tốt.', N'Môi trường đa quốc gia chuyên nghiệp.', N'BA, Agile, Jira', N'Mid-Level', N'Full-time', 16000000, 25000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- Sendo (130)
(130, 4, N'Fullstack Web Engineer (Node/React)', N'Tối ưu các phân hệ giỏ hàng và tích điểm Sen Đỏ.', N'NodeJS, ReactJS, MongoDB, Microservices.', N'Cung cấp cơm trưa miễn phí, thưởng dự án lớn.', N'Node.js, ReactJS, MongoDB', N'Mid-Level', N'Hybrid', 20000000, 35000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(130, 11, N'QA Manual Tester (Web/App)', N'Kiểm tra tính năng và đảm bảo giao diện đạt yêu cầu.', N'Lập tài liệu testcase, test API cơ bản.', N'Đồng nghiệp thân thiện, sếp tâm lý.', N'Manual Test, Testcase', N'Junior', N'Full-time', 10000000, 15000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- VNPAY (131)
(131, 1, N'Senior Java API Developer', N'Xây dựng API bảo mật cho hệ thống cổng thanh toán VNPAY-QR.', N'Java Core, Spring Boot, Oracle DB, Security.', N'Lương net cạnh tranh, chế độ đãi ngộ xuất sắc.', N'Java, Spring Boot, Security', N'Senior', N'Full-time', 30000000, 55000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(131, 5, N'Mobile React Native Engineer', N'Phát triển tính năng ví điện tử VNPAY trên di động.', N'React Native, JavaScript, Redux Saga.', N'Review lương đột xuất theo hiệu suất.', N'React Native, Mobile', N'Mid-Level', N'Full-time', 20000000, 35000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Viettel IDC (132)
(132, 8, N'System Cloud Administrator', N'Quản trị máy chủ ảo hóa và hệ thống đám mây Cloud.', N'VMware, OpenStack, Linux, Networking.', N'Lương tháng 14, gói chăm sóc sức khoẻ VIP.', N'System, Linux, Cloud', N'Mid-Level', N'Full-time', 22000000, 35000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(132, 18, N'Data Center Engineer', N'Vận hành cơ sở hạ tầng lưu trữ và mạng kết nối.', N'Quản trị mạng CCNA/CCNP, kiểm tra tủ rack.', N'Làm việc môi trường công nghệ viễn thông lớn nhất.', N'Network, Cisco, Datacenter', N'Junior', N'Full-time', 12000000, 18000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- VNPT-IT (133)
(133, 1, N'Golang Backend Engineer', N'Xây dựng hệ sinh thái ứng dụng thông minh chính phủ.', N'Golang, RESTful API, PostgreSQL, Redis.', N'Môi trường ổn định, công việc lâu dài.', N'Golang, PostgreSQL, Redis', N'Mid-Level', N'Full-time', 20000000, 32000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(133, 3, N'Angular Frontend Developer', N'Phát triển giao diện cổng dịch vụ công trực tuyến.', N'Angular v14+, RxJS, Tailwind CSS.', N'Đầy đủ chế độ bảo hiểm theo quy định nhà nước.', N'Angular, CSS, HTML', N'Mid-Level', N'Full-time', 15000000, 24000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Mobifone IT (134)
(134, 1, N'Oracle Database Administrator (DBA)', N'Tối ưu và quản trị cơ sở dữ liệu viễn thông khổng lồ.', N'Oracle DB, PL/SQL, Backup & Recovery, Performance Tuning.', N'Thưởng ngày lễ tết lớn, du lịch hè sang trọng.', N'Oracle, SQL, Database', N'Senior', N'Full-time', 28000000, 48000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(134, 11, N'QC Manual Test Engineer', N'Kiểm soát chất lượng phần mềm dịch vụ viễn thông.', N'Lập kịch bản testcase chi tiết, hiểu quy trình phần mềm.', N'Ăn trưa miễn phí tại căng tin công ty.', N'Manual Test, Testcase', N'Junior', N'Full-time', 11000000, 16000000, 4, '2026-09-30', 'APPROVED', GETDATE()),

-- Appota (135)
(135, 4, N'Fullstack Developer (Python/React)', N'Phát triển các cổng thanh toán và game của Appota.', N'Python, Django/FastAPI, ReactJS, PostgreSQL.', N'Review lương định kỳ, tặng quà sinh nhật.', N'Python, ReactJS, SQL', N'Mid-Level', N'Hybrid', 18000000, 30000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(135, 5, N'Android Kotlin Developer (Game SDK)', N'Phát triển SDK kết nối quảng cáo và thanh toán game.', N'Kotlin, Java, Gradle, API Integrations.', N'Hỗ trợ thiết bị kiểm thử phong phú.', N'Android, Kotlin, SDK', N'Mid-Level', N'Full-time', 18000000, 28000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- Gotadi (136)
(136, 1, N'NodeJS Web API Engineer', N'Xây dựng cổng kết nối với các hãng hàng không.', N'NodeJS, NestJS, REST/SOAP, MongoDB.', N'Ưu đãi vé máy bay du lịch cho nhân viên.', N'NodeJS, NestJS, Database', N'Mid-Level', N'Full-time', 16000000, 26000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(136, 14, N'Product Owner (Travel App)', N'Định hình hướng đi cho ứng dụng du lịch đặt vé Gotadi.', N'Kinh nghiệm PO/PM, am hiểu thiết kế trải nghiệm người dùng.', N'Mức thu nhập hấp dẫn, cơ hội nâng cao nghiệp vụ.', N'PO, Product, Agile', N'Senior', N'Full-time', 30000000, 50000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- ELSA Speak (137)
(137, 19, N'AI Speech Recognition Scientist', N'Nghiên cứu công nghệ nhận dạng giọng nói nâng cao.', N'Python, Deep Learning, Speech Recognition, PyTorch.', N'Được làm việc với Giáo sư đầu ngành, cổ phần ưu đãi.', N'Python, PyTorch, Speech AI', N'Senior', N'Full-time', 40000000, 80000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(137, 3, N'Frontend Web Developer (Next.js)', N'Phát triển cổng học tập trực tuyến của ELSA.', N'ReactJS, Next.js, Tailwind CSS, TypeScript.', N'Giờ giấc linh hoạt, hỗ trợ 100% làm việc từ xa.', N'ReactJS, Next.js, Tailwind', N'Mid-Level', N'Remote', 20000000, 32000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Gapo (138)
(138, 4, N'Fullstack JavaScript Developer', N'Phát triển ứng dụng GapoWork cho doanh nghiệp.', N'ReactJS, NodeJS, Webpack, Redis.', N'Môi trường khởi nghiệp trẻ trung, năng động.', N'ReactJS, NodeJS, CSS', N'Mid-Level', N'Full-time', 17000000, 28000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(138, 11, N'QA Manual QC Analyst', N'Đảm bảo các bản cập nhật app chạy mượt mà, không bug.', N'Lập kế hoạch testcase chi tiết, kiểm thử hồi quy.', N'Mức đãi ngộ cạnh tranh, đóng bảo hiểm xã hội đầy đủ.', N'QA, Manual Test, App', N'Mid-Level', N'Full-time', 12000000, 18000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Ahamove (139)
(139, 1, N'Senior Golang Developer (Real-time)', N'Phát triển hệ thống định tuyến đơn hàng tức thời.', N'Golang, Kafka, Redis, Microservices.', N'Thưởng hiệu quả hàng tháng, trà chiều buffet.', N'Golang, Kafka, Redis', N'Senior', N'Full-time', 30000000, 50000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(139, 18, N'Data Analyst (Logistics)', N'Phân tích các chỉ số vận hành và hành vi shipper.', N'Python, SQL, Tableau/PowerBI, Excel tốt.', N'Được tiếp cận kho dữ liệu logistics lớn nhất.', N'SQL, PowerBI, Python', N'Mid-Level', N'Full-time', 16000000, 26000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Be Group (140)
(140, 1, N'Backend Engineer (C# / .NET)', N'Xây dựng các API thanh toán xe và gọi món.', N'C#, .NET Core, SQL Server, Microservices.', N'Review lương mỗi năm, khám sức khỏe quốc tế.', N'C#, .NET Core, SQL', N'Mid-Level', N'Full-time', 18000000, 30000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(140, 7, N'Flutter Cross-platform Developer', N'Phát triển ứng dụng gọi xe đa nền tảng.', N'Flutter, Dart, Bloc/Provider, Git.', N'Cơ hội onsite hoặc làm việc hybrid cực thích.', N'Flutter, Dart, Git', N'Mid-Level', N'Hybrid', 18000000, 28000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- Trusting Social (141)
(141, 19, N'Machine Learning Engineer (AI)', N'Xây dựng thuật toán chấm điểm tín dụng tài chính.', N'Python, R, TensorFlow, Spark, Machine Learning.', N'Đóng bảo hiểm toàn diện full lương net, môi trường tuyệt vời.', N'Python, Machine Learning, AI', N'Senior', N'Full-time', 35000000, 65000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(141, 1, N'Backend Developer (Python/SQL)', N'Xây dựng các dịch vụ API cung cấp dữ liệu điểm.', N'Python, FastAPI, SQL Server, ETL pipelines.', N'Cung cấp thiết bị làm việc Macbook Pro mớii nhất.', N'Python, SQL, API', N'Mid-Level', N'Full-time', 18000000, 30000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- Garena (142)
(142, 1, N'C++ Backend System Engineer', N'Tối ưu máy chủ phòng game và kết nối mạng.', N'C++, Network Programming, Multithreading.', N'Trải nghiệm chơi game đỉnh cao, văn phòng hiện đại.', N'C++, Network, Multithreading', N'Senior', N'Full-time', 35000000, 60000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(142, 10, N'IT Support (Helpdesk)', N'Quản trị hệ thống máy chủ vận hành giải đấu game.', N'CCNA, quản trị Windows Server, hỗ trợ tốt.', N'Được tài trợ tham gia các giải đấu game lớn.', N'Network, Server, support', N'Junior', N'Full-time', 10000000, 15000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Vexere (143)
(143, 1, N'ASP.NET Core Web Engineer', N'Xây dựng phân hệ đặt vé xe khách và quản trị nhà xe.', N'C#, .NET Core, SQL Server, Javascript.', N'Môi trường Product chuyên nghiệp, tôn trọng cá nhân.', N'C#, .NET Core, SQL', N'Mid-Level', N'Full-time', 17000000, 28000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(143, 11, N'QC Manual Test Specialist', N'Đảm bảo chất lượng các ứng dụng đặt vé trực tuyến.', N'Manual Testing, API test với Postman, tư duy logic.', N'Cấp laptop, đầy đủ phúc lợi, du lịch hè.', N'Manual Test, Postman', N'Mid-Level', N'Full-time', 12000000, 18000000, 4, '2026-09-30', 'APPROVED', GETDATE()),

-- Luxstay (144)
(144, 3, N'Frontend Developer (React/NextJS)', N'Phát triển ứng dụng kết nối đặt phòng homestay.', N'ReactJS, Next.js, HTML/CSS, Tailwind.', N'Ưu đãi voucher nghỉ dưỡng trên toàn hệ thống homestay.', N'ReactJS, Next.js, CSS', N'Mid-Level', N'Hybrid', 16000000, 26000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(144, 11, N'QC Tester (Manual/Mobile)', N'Kiểm thử ứng dụng Luxstay trên nền tảng Android/iOS.', N'Lập kịch bản testcase chi tiết, báo cáo bug.', N'Cơ hội học hỏi và phát triển lên Automation.', N'Manual Test, Mobile Test', N'Junior', N'Full-time', 11000000, 16000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- JupViec (145)
(145, 1, N'PHP backend Developer', N'Nâng cấp API kết nối ứng dụng JupViec.', N'PHP, Laravel, MySQL, RESTful API.', N'Lương net, đóng bảo hiểm đầy đủ, teambuilding.', N'PHP, Laravel, MySQL', N'Mid-Level', N'Full-time', 15000000, 24000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(145, 7, N'Mobile Developer (Flutter)', N'Tối ưu ứng dụng người giúp việc trên di động.', N'Flutter, Dart, Bloc Pattern.', N'Cấp laptop, giờ giấc linh hoạt thoải mái.', N'Flutter, Dart', N'Mid-Level', N'Full-time', 15000000, 25000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Edumall (146)
(146, 1, N'Java backend Developer (Edtech)', N'Phát triển cổng học tập trực tuyến.', N'Java Core, Spring Boot, PostgreSQL.', N'Được học miễn phí tất cả khóa học trên Edumall.', N'Java, Spring Boot, SQL', N'Mid-Level', N'Full-time', 16000000, 26000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(146, 3, N'Frontend Web Developer (ReactJS)', N'Phát triển giao diện trang chủ mua khoá học.', N'ReactJS, CSS3, HTML5, Javascript.', N'Văn phòng cực đẹp, đồng nghiệp thân thiện.', N'ReactJS, HTML, CSS', N'Junior', N'Full-time', 11000000, 16000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Topica (147)
(147, 1, N'ASP.NET Backend Developer', N'Phát triển hệ thống học trực tuyến Topica Native.', N'C#, .NET MVC, SQL Server.', N'Thưởng ngày lễ tết lớn, cơ hội tăng lương tốt.', N'C#, .NET, SQL Server', N'Mid-Level', N'Full-time', 18000000, 28000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(147, 3, N'Frontend Developer (ReactJS)', N'Phát triển giao diện lớp học trực tuyến.', N'ReactJS, WebRTC, Tailwind CSS, ES6.', N'Lương tháng 13 đầy đủ, cơ hội thăng tiến.', N'ReactJS, CSS, WebRTC', N'Mid-Level', N'Full-time', 16000000, 25000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Waka (148)
(148, 1, N'Python backend Developer (Book app)', N'Phát triển API đọc sách trực tuyến cho ứng dụng di động.', N'Python, Flask, MySQL, Redis.', N'Được đọc miễn phí hàng ngàn đầu sách hot.', N'Python, MySQL, Redis', N'Mid-Level', N'Full-time', 15000000, 25000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(148, 11, N'Manual QC Tester (Book)', N'Kiểm thử ứng dụng Waka trên Web và Mobile.', N'Viết testcase chi tiết, tư duy kiểm thử tốt.', N'Môi trường làm việc năng động, sáng tạo.', N'Manual Test, Testcase', N'Junior', N'Full-time', 10000000, 15000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- VinBigData (149)
(149, 19, N'Senior Computer Vision Researcher', N'Nghiên cứu giải thuật xử lý ảnh thông minh cho xe điện.', N'Python, C++, OpenCV, PyTorch, TensorFlow.', N'Chế độ đãi ngộ vượt trội, làm việc trực tiếp với chuyên gia.', N'AI, Computer Vision, C++', N'Senior', N'Full-time', 35000000, 65000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(149, 18, N'Data Infrastructure Engineer', N'Xây dựng kiến trúc kho dữ liệu cho camera thông minh.', N'Spark, Python, Big Data, Kubernetes.', N'Trang thiết bị hiện đại hàng đầu Việt Nam.', N'Python, Spark, Cloud', N'Senior', N'Full-time', 30000000, 50000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- VNG (150)
(150, 1, N'Senior Backend Engineer (Zalo Pay)', N'Thiết kế kiến trúc API chịu tải cực lớn.', N'Java Core, Go, Redis, Microservices.', N'Môi trường cực tốt, cơm trưa buffet miễn phí tại căn tin.', N'Java, Golang, Redis', N'Senior', N'Full-time', 35000000, 60000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(150, 7, N'Mobile iOS Developer (Swift)', N'Phát triển các tính năng nhắn tin trong ứng dụng Zalo.', N'Swift, Objective-C, UIKit, Memory management.', N'Thu nhập khủng, cổ phiếu thưởng ESOP hàng năm.', N'iOS, Swift, UIKit', N'Senior', N'Full-time', 35000000, 55000000, 3, '2026-09-30', 'APPROVED', GETDATE());

-- =====================================================
-- 4. ÁNH XẠ JOBS VÀO BẢNG CHÂN TRỊ TỈNH THÀNH (job_post_province)
-- =====================================================
PRINT N'Đang ánh xạ tỉnh thành cho 50 jobs bổ sung...';

DECLARE @ProvincesTable TABLE (
    Id INT IDENTITY(1, 1),
    ProvinceId INT
);

-- Danh sách 50 ID tỉnh thành không bị trùng lắp (tương ứng thứ tự 50 Jobs ở trên)
INSERT INTO @ProvincesTable (ProvinceId) VALUES
(3), (49), (4), (11), (5), (55), (62), (8), (40), (56),
(41), (57), (32), (14), (22), (7), (39), (58), (36), (33),
(17), (46), (47), (1), (2), (28), (31), (3), (49), (4),
(11), (5), (55), (62), (8), (40), (56), (41), (57), (32),
(14), (22), (7), (39), (58), (36), (33), (17), (46), (47);

DECLARE @CurrentRow INT = 1;
DECLARE @TotalInserted INT = (SELECT COUNT(*) FROM @InsertedJobs);
DECLARE @TargetJobId INT;
DECLARE @TargetProvinceId INT;

WHILE @CurrentRow <= @TotalInserted
BEGIN
    SELECT @TargetJobId = JobId FROM @InsertedJobs WHERE RowNumber = @CurrentRow;
    SELECT @TargetProvinceId = ProvinceId FROM @ProvincesTable WHERE Id = @CurrentRow;

    INSERT INTO dbo.job_post_province (job_id, province_id)
    VALUES (@TargetJobId, @TargetProvinceId);

    -- =====================================================
    -- 5. THÊM TECH STACK MẪU CHO MỖI JOB (job_tech_stack)
    -- =====================================================
    IF @CurrentRow % 5 = 1 
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 3), (@TargetJobId, 4), (@TargetJobId, 17); -- .NET / C# / SQL Server
    ELSE IF @CurrentRow % 5 = 2 
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 7), (@TargetJobId, 10); -- ReactJS / TypeScript
    ELSE IF @CurrentRow % 5 = 3 
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 1), (@TargetJobId, 2); -- Java / Spring Boot
    ELSE IF @CurrentRow % 5 = 4 
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 5), (@TargetJobId, 18); -- Python / PostgreSQL
    ELSE 
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 11), (@TargetJobId, 13); -- Flutter / Docker

    SET @CurrentRow = @CurrentRow + 1;
END;

PRINT N'Hoàn thành thêm dữ liệu bổ sung thành công!';
GO
