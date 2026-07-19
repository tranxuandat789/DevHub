USE DevHub;
GO
SET NOCOUNT ON;

-- =====================================================
-- 1. CẬP NHẬT TÊN TỈNH THÀNH TIẾNG VIỆT CHUẨN (SỬA LỖI FONT TRƯỚC ĐÓ)
-- =====================================================
PRINT N'Đang cập nhật danh mục tỉnh thành...';
UPDATE dbo.province SET province_name = N'Hà Nội' WHERE province_id = 1;
UPDATE dbo.province SET province_name = N'Hồ Chí Minh' WHERE province_id = 2;
UPDATE dbo.province SET province_name = N'Hải Phòng' WHERE province_id = 3;
UPDATE dbo.province SET province_name = N'Đà Nẵng' WHERE province_id = 4;
UPDATE dbo.province SET province_name = N'Cần Thơ' WHERE province_id = 5;
UPDATE dbo.province SET province_name = N'An Giang' WHERE province_id = 6;
UPDATE dbo.province SET province_name = N'Bà Rịa - Vũng Tàu' WHERE province_id = 7;
UPDATE dbo.province SET province_name = N'Bắc Giang' WHERE province_id = 8;
UPDATE dbo.province SET province_name = N'Bắc Kạn' WHERE province_id = 9;
UPDATE dbo.province SET province_name = N'Bạc Liêu' WHERE province_id = 10;
UPDATE dbo.province SET province_name = N'Bắc Ninh' WHERE province_id = 11;
UPDATE dbo.province SET province_name = N'Bến Tre' WHERE province_id = 12;
UPDATE dbo.province SET province_name = N'Bình Định' WHERE province_id = 13;
UPDATE dbo.province SET province_name = N'Bình Dương' WHERE province_id = 14;
UPDATE dbo.province SET province_name = N'Bình Phước' WHERE province_id = 15;
UPDATE dbo.province SET province_name = N'Bình Thuận' WHERE province_id = 16;
UPDATE dbo.province SET province_name = N'Cà Mau' WHERE province_id = 17;
UPDATE dbo.province SET province_name = N'Cao Bằng' WHERE province_id = 18;
UPDATE dbo.province SET province_name = N'Đắk Lắk' WHERE province_id = 19;
UPDATE dbo.province SET province_name = N'Đắk Nông' WHERE province_id = 20;
UPDATE dbo.province SET province_name = N'Điện Biên' WHERE province_id = 21;
UPDATE dbo.province SET province_name = N'Đồng Nai' WHERE province_id = 22;
UPDATE dbo.province SET province_name = N'Đồng Tháp' WHERE province_id = 23;
UPDATE dbo.province SET province_name = N'Gia Lai' WHERE province_id = 24;
UPDATE dbo.province SET province_name = N'Hà Giang' WHERE province_id = 25;
UPDATE dbo.province SET province_name = N'Hà Nam' WHERE province_id = 26;
UPDATE dbo.province SET province_name = N'Hà Tĩnh' WHERE province_id = 27;
UPDATE dbo.province SET province_name = N'Hải Dương' WHERE province_id = 28;
UPDATE dbo.province SET province_name = N'Hậu Giang' WHERE province_id = 29;
UPDATE dbo.province SET province_name = N'Hòa Bình' WHERE province_id = 30;
UPDATE dbo.province SET province_name = N'Hưng Yên' WHERE province_id = 31;
UPDATE dbo.province SET province_name = N'Khánh Hòa' WHERE province_id = 32;
UPDATE dbo.province SET province_name = N'Kiên Giang' WHERE province_id = 33;
UPDATE dbo.province SET province_name = N'Kon Tum' WHERE province_id = 34;
UPDATE dbo.province SET province_name = N'Lai Châu' WHERE province_id = 35;
UPDATE dbo.province SET province_name = N'Lâm Đồng' WHERE province_id = 36;
UPDATE dbo.province SET province_name = N'Lạng Sơn' WHERE province_id = 37;
UPDATE dbo.province SET province_name = N'Lào Cai' WHERE province_id = 38;
UPDATE dbo.province SET province_name = N'Long An' WHERE province_id = 39;
UPDATE dbo.province SET province_name = N'Nam Định' WHERE province_id = 40;
UPDATE dbo.province SET province_name = N'Nghệ An' WHERE province_id = 41;
UPDATE dbo.province SET province_name = N'Ninh Bình' WHERE province_id = 42;
UPDATE dbo.province SET province_name = N'Ninh Thuận' WHERE province_id = 43;
UPDATE dbo.province SET province_name = N'Phú Thọ' WHERE province_id = 44;
UPDATE dbo.province SET province_name = N'Phú Yên' WHERE province_id = 45;
UPDATE dbo.province SET province_name = N'Quảng Bình' WHERE province_id = 46;
UPDATE dbo.province SET province_name = N'Quảng Nam' WHERE province_id = 47;
UPDATE dbo.province SET province_name = N'Quảng Ngãi' WHERE province_id = 48;
UPDATE dbo.province SET province_name = N'Quảng Ninh' WHERE province_id = 49;
UPDATE dbo.province SET province_name = N'Quảng Trị' WHERE province_id = 50;
UPDATE dbo.province SET province_name = N'Sóc Trăng' WHERE province_id = 51;
UPDATE dbo.province SET province_name = N'Sơn La' WHERE province_id = 52;
UPDATE dbo.province SET province_name = N'Tây Ninh' WHERE province_id = 53;
UPDATE dbo.province SET province_name = N'Thái Bình' WHERE province_id = 54;
UPDATE dbo.province SET province_name = N'Thái Nguyên' WHERE province_id = 55;
UPDATE dbo.province SET province_name = N'Thanh Hóa' WHERE province_id = 56;
UPDATE dbo.province SET province_name = N'Thừa Thiên Huế' WHERE province_id = 57;
UPDATE dbo.province SET province_name = N'Tiền Giang' WHERE province_id = 58;
UPDATE dbo.province SET province_name = N'Trà Vinh' WHERE province_id = 59;
UPDATE dbo.province SET province_name = N'Tuyên Quang' WHERE province_id = 60;
UPDATE dbo.province SET province_name = N'Vĩnh Long' WHERE province_id = 61;
UPDATE dbo.province SET province_name = N'Vĩnh Phúc' WHERE province_id = 62;
UPDATE dbo.province SET province_name = N'Yên Bái' WHERE province_id = 63;

-- Cập nhật địa chỉ công ty hiện tại
UPDATE dbo.company SET company_address = N'Hà Nội' WHERE company_address = N'Ha N?i';
UPDATE dbo.company SET company_address = N'Hồ Chí Minh' WHERE company_address = N'H? Chi Minh';
UPDATE dbo.company SET company_address = N'Đà Nẵng' WHERE company_address = N'?a N?ng';

-- =====================================================
-- 2. XÓA CÁC CÔNG TY VÀ JOB MOCK TRÙNG NẾU CÓ ĐỂ CHẠY LẠI KHÔNG LỖI
-- =====================================================
PRINT N'Đang dọn dẹp dữ liệu mock cũ...';
DELETE FROM dbo.job_post_province WHERE job_id IN (SELECT job_id FROM dbo.job_post WHERE company_id BETWEEN 101 AND 125);
DELETE FROM dbo.job_tech_stack WHERE job_id IN (SELECT job_id FROM dbo.job_post WHERE company_id BETWEEN 101 AND 125);
DELETE FROM dbo.job_post WHERE company_id BETWEEN 101 AND 125;
DELETE FROM dbo.company WHERE company_id BETWEEN 101 AND 125;

-- =====================================================
-- 3. THÊM 25 CÔNG TY KHÁC NHAU (ID 101 ĐẾN 125)
-- =====================================================
PRINT N'Đang thêm 25 công ty mới...';
SET IDENTITY_INSERT dbo.company ON;
INSERT INTO dbo.company (company_id, company_name, company_address, company_description, website, industry, tax_code, is_verified, profile_completion, status)
VALUES
(101, N'Công ty Cổ phần MISA', N'Hà Nội', N'MISA là công ty phần mềm kế toán hàng đầu Việt Nam.', 'https://misa.com.vn', N'Phần mềm', '0101230101', 1, 80, 'APPROVED'),
(102, N'Công ty Cổ phần Haravan', N'Hồ Chí Minh', N'Haravan cung cấp giải pháp bán lẻ và thương mại điện tử.', 'https://haravan.com', N'Thương mại điện tử', '0101230102', 1, 80, 'APPROVED'),
(103, N'Công ty TNHH Tiki', N'Hồ Chí Minh', N'Tiki là một trong những sàn thương mại điện tử lớn nhất Việt Nam.', 'https://tiki.vn', N'Thương mại điện tử', '0101230103', 1, 85, 'APPROVED'),
(104, N'Công ty Cổ phần Shopee', N'Hồ Chí Minh', N'Shopee là nền tảng thương mại điện tử hàng đầu Đông Nam Á.', 'https://shopee.vn', N'Thương mại điện tử', '0101230104', 1, 85, 'APPROVED'),
(105, N'Công ty TNHH Grab Việt Nam', N'Hồ Chí Minh', N'Grab cung cấp dịch vụ vận chuyển và giao nhận hàng đầu.', 'https://grab.com/vn', N'Dịch vụ vận chuyển', '0101230105', 1, 80, 'APPROVED'),
(106, N'Công ty Cổ phần OneMount Group', N'Hà Nội', N'OneMount Group kiến tạo hệ sinh thái công nghệ lớn nhất Việt Nam.', 'https://onemount.com', N'Công nghệ', '0101230106', 1, 90, 'APPROVED'),
(107, N'Công ty Cổ phần Base.vn', N'Hà Nội', N'Base.vn là nền tảng quản trị doanh nghiệp phổ biến nhất.', 'https://base.vn', N'Phần mềm SaaS', '0101230107', 1, 80, 'APPROVED'),
(108, N'Công ty Cổ phần TopCV Việt Nam', N'Hà Nội', N'TopCV là nền tảng tuyển dụng hàng đầu Việt Nam.', 'https://topcv.vn', N'Nhân sự', '0101230108', 1, 85, 'APPROVED'),
(109, N'Công ty Cổ phần Giao Hàng Tiết Kiệm', N'Hà Nội', N'Giao Hàng Tiết Kiệm cung cấp dịch vụ logistics chuyên nghiệp.', 'https://ghtk.vn', N'Logistics', '0101230109', 1, 80, 'APPROVED'),
(110, N'Công ty Cổ phần Giao Hàng Nhanh', N'Hồ Chí Minh', N'Giao Hàng Nhanh là đối tác giao hàng tin cậy của mọi doanh nghiệp.', 'https://ghn.vn', N'Logistics', '0101230110', 1, 80, 'APPROVED'),
(111, N'Công ty TNHH KMS Technology', N'Hồ Chí Minh', N'KMS Technology là doanh nghiệp phát triển phần mềm chất lượng cao.', 'https://kms-technology.com', N'Phần mềm', '0101230111', 1, 80, 'APPROVED'),
(112, N'Công ty TNHH DEK Technologies', N'Hồ Chí Minh', N'DEK Technologies chuyên về các giải pháp phần mềm nhúng viễn thông.', 'https://dektech.com.au', N'Phần mềm viễn thông', '0101230112', 1, 80, 'APPROVED'),
(113, N'Công ty Cổ phần SmartOSC', N'Hà Nội', N'SmartOSC là đối tác công nghệ thương mại điện tử toàn cầu.', 'https://smartosc.com', N'Phần mềm', '0101230113', 1, 80, 'APPROVED'),
(114, N'Công ty Cổ phần Rikkeisoft', N'Hà Nội', N'Rikkeisoft chuyên cung cấp dịch vụ CNTT cho thị trường Nhật Bản.', 'https://rikkeisoft.com', N'Phần mềm', '0101230114', 1, 85, 'APPROVED'),
(115, N'Công ty Cổ phần Luvina Software', N'Hà Nội', N'Luvina chuyên đào tạo và phát triển phần mềm cho đối tác Nhật Bản.', 'https://luvina.net', N'Phần mềm', '0101230115', 1, 80, 'APPROVED'),
(116, N'Công ty Cổ phần Sun* Asterisk', N'Hà Nội', N'Sun* Asterisk phát triển sản phẩm công nghệ sáng tạo toàn cầu.', 'https://sun-asterisk.vn', N'Công nghệ', '0101230116', 1, 90, 'APPROVED'),
(117, N'Công ty TNHH VTI Việt Nam', N'Hà Nội', N'VTI cung cấp giải pháp CNTT toàn diện cho thị trường Nhật Bản.', 'https://vti.com.vn', N'Phần mềm', '0101230117', 1, 80, 'APPROVED'),
(118, N'Công ty Cổ phần SotaTek', N'Hà Nội', N'SotaTek là công ty phát triển Blockchain hàng đầu Đông Nam Á.', 'https://sotatek.com', N'Blockchain & Software', '0101230118', 1, 85, 'APPROVED'),
(119, N'Công ty Cổ phần NextPay', N'Hà Nội', N'NextPay là mạng lưới thanh toán toàn diện của NextTech Group.', 'https://nextpay.vn', N'Fintech', '0101230119', 1, 80, 'APPROVED'),
(120, N'Công ty Cổ phần KiotViet', N'Hà Nội', N'KiotViet là phần mềm quản lý bán hàng phổ biến nhất Việt Nam.', 'https://kiotviet.vn', N'Phần mềm SaaS', '0101230120', 1, 85, 'APPROVED'),
(121, N'Công ty Cổ phần Sentifi Việt Nam', N'Hồ Chí Minh', N'Sentifi phân tích dữ liệu tài chính toàn cầu dựa trên AI.', 'https://sentifi.com', N'Công nghệ tài chính', '0101230121', 1, 80, 'APPROVED'),
(122, N'Công ty Cổ phần Cốc Cốc', N'Hà Nội', N'Cốc Cốc phát triển trình duyệt và công cụ tìm kiếm của người Việt.', 'https://coccoc.com', N'Internet', '0101230122', 1, 85, 'APPROVED'),
(123, N'Công ty TNHH Gear Inc', N'Đà Nẵng', N'Gear Inc chuyên về kiểm duyệt nội dung và hỗ trợ khách hàng toàn cầu.', 'https://gearinc.com', N'BPO & Game', '0101230123', 1, 80, 'APPROVED'),
(124, N'Công ty Cổ phần Axon Active', N'Đà Nẵng', N'Axon Active phát triển phần mềm theo mô hình Agile chuyên nghiệp.', 'https://axonactive.com', N'Phần mềm', '0101230124', 1, 80, 'APPROVED'),
(125, N'Công ty Cổ phần Enclave', N'Đà Nẵng', N'Enclave chuyên cung cấp dịch vụ gia công phần mềm chất lượng cao.', 'https://enclave.vn', N'Phần mềm', '0101230125', 1, 80, 'APPROVED');
SET IDENTITY_INSERT dbo.company OFF;

-- =====================================================
-- 4. THÊM 50 JOBS (2 JOB CHO MỖI CÔNG TY)
-- =====================================================
PRINT N'Đang thêm 50 tin tuyển dụng mới...';

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
-- MISA (101)
(101, 1, N'Senior Backend Developer (.NET)', N'Phát triển các dịch vụ tài chính quy mô lớn.', N'C#, .NET Core, SQL Server, 4 năm kinh nghiệm.', N'Lương tháng 13, bảo hiểm sức khỏe PVI.', N'C#, .NET, SQL Server', N'Senior', N'Full-time', 25000000, 45000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(101, 3, N'Frontend Web Developer (ReactJS)', N'Phát triển giao diện ứng dụng kế toán trực tuyến.', N'ReactJS, HTML5, CSS3, ES6, Redux.', N'Laptop làm việc chất lượng cao, du lịch hè.', N'ReactJS, HTML, CSS', N'Mid-Level', N'Full-time', 15000000, 25000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Haravan (102)
(102, 4, N'Fullstack Developer (NodeJS & React)', N'Xây dựng các API và hệ thống quản trị bán hàng.', N'NodeJS, ReactJS, MongoDB, RESTful API.', N'Thưởng dự án hấp dẫn, lộ trình thăng tiến rõ ràng.', N'Node.js, ReactJS, MongoDB', N'Mid-Level', N'Hybrid', 18000000, 30000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(102, 11, N'QA Automation Engineer (Selenium)', N'Viết script test tự động cho ứng dụng thương mại.', N'Selenium, Java/Python, CI/CD, viết testcase.', N'Được đào tạo bài bản, trợ cấp chứng chỉ.', N'QA, Selenium, Test', N'Junior', N'Full-time', 12000000, 18000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- Tiki (103)
(103, 1, N'Backend Engineer (Java/Spring Boot)', N'Tối ưu hóa giỏ hàng và thanh toán trên sàn Tiki.', N'Java, Spring Boot, MySQL, Kafka, Redis.', N'Môi trường năng động, thưởng cuối năm cực tốt.', N'Java, Spring Boot, MySQL', N'Senior', N'Full-time', 30000000, 50000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(103, 8, N'DevOps Cloud Engineer (AWS)', N'Quản trị cơ sở hạ tầng đám mây và tối ưu CI/CD.', N'AWS, Kubernetes, Docker, Terraform, Linux.', N'Phúc lợi ngập tràn, miễn phí bữa trưa.', N'AWS, Kubernetes, Docker', N'Mid-Level', N'Remote', 25000000, 40000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Shopee (104)
(104, 5, N'Mobile iOS Developer (Swift)', N'Phát triển ứng dụng Shopee mượt mà trên iOS.', N'Swift, SwiftUI, UIKit, có app trên Store.', N'Mức lương đột phá, văn phòng chuẩn quốc tế.', N'iOS, Swift, UIKit', N'Senior', N'Full-time', 35000000, 60000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(104, 14, N'IT Business Analyst (Fintech)', N'Khảo sát yêu cầu nghiệp vụ và viết SRS tài liệu.', N'Kinh nghiệm BA trên 2 năm, hiểu về thanh toán.', N'Được làm việc với các chuyên gia nước ngoài.', N'BA, SQL, Agile', N'Mid-Level', N'Hybrid', 18000000, 30000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Grab (105)
(105, 18, N'Senior Data Engineer (Big Data)', N'Xây dựng đường ống xử lý dữ liệu phục vụ giao vận.', N'Spark, Scala, Python, Hadoop, Hive.', N'Lương net 100%, bảo hiểm sức khỏe cao cấp toàn gia đình.', N'Spark, Python, Big Data', N'Senior', N'Full-time', 40000000, 70000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(105, 2, N'Junior Python Backend Developer', N'Hỗ trợ bảo trì các dịch vụ backend phục vụ đặt xe.', N'Python, Flask/Django, SQL cơ bản.', N'Được đào tạo chi tiết từ các mentor hàng đầu.', N'Python, Django, SQL', N'Junior', N'Full-time', 12000000, 18000000, 5, '2026-09-30', 'APPROVED', GETDATE()),

-- OneMount (106)
(106, 1, N'Golang Backend Engineer', N'Xây dựng hệ thống ví điện tử VinID.', N'Golang, Redis, Kafka, Microservices.', N'Thưởng hiệu quả công việc, cấp Macbook Pro M3.', N'Golang, Redis, Kafka', N'Mid-Level', N'Full-time', 22000000, 38000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(106, 3, N'Frontend Developer (VueJS)', N'Phát triển ứng dụng web Vinshop cho đại lý.', N'VueJS, Pinia, Tailwind CSS, JavaScript.', N'Lương net, đánh giá tăng lương 2 lần/năm.', N'VueJS, CSS, HTML', N'Mid-Level', N'Hybrid', 16000000, 26000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Base.vn (107)
(107, 1, N'NodeJS Web Developer', N'Phát triển hệ thống Base Workflow và Base Cabin.', N'NodeJS, Express/NestJS, PostgreSQL.', N'Môi trường trẻ trung, cởi mở, không OT.', N'Node.js, PostgreSQL', N'Mid-Level', N'Full-time', 16000000, 28000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(107, 20, N'UI/UX Designer', N'Thiết kế giao diện các sản phẩm quản trị Base.', N'Figma, Sketch, hiểu biết về UX nghiên cứu.', N'Được tự do sáng tạo thiết kế sản phẩm hàng đầu.', N'UI/UX, Figma', N'Mid-Level', N'Full-time', 14000000, 22000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- TopCV (108)
(108, 1, N'PHP Backend Engineer (Laravel)', N'Nâng cấp hệ thống lõi kết nối ứng viên và nhà tuyển dụng.', N'PHP, Laravel, MySQL, Redis, Solr/ElasticSearch.', N'Lương net, thưởng quý hấp dẫn theo hiệu quả công ty.', N'PHP, Laravel, MySQL', N'Mid-Level', N'Full-time', 15000000, 25000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(108, 3, N'Frontend Developer (React/Next.js)', N'Tối ưu hóa giao diện viết CV trực tuyến.', N'Next.js, Tailwind CSS, TypeScript, SEO tối ưu.', N'Du lịch công ty định kỳ, môi trường siêu vui.', N'Next.js, TypeScript, CSS', N'Mid-Level', N'Hybrid', 17000000, 28000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Giao Hàng Tiết Kiệm (109)
(109, 1, N'Senior Java Developer (Logistics Core)', N'Tối ưu hệ thống điều vận hàng triệu đơn hàng/ngày.', N'Java Core, Spring Boot, MySQL, Multi-threading.', N'Thương lượng lương theo năng lực, đóng bảo hiểm đầy đủ.', N'Java, Spring Boot, SQL', N'Senior', N'Full-time', 28000000, 45000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(109, 18, N'Data Scientist (Optimization)', N'Nghiên cứu mô hình phân tuyến xe giao hàng tự động.', N'Python, SQL, Machine Learning, thuật toán tối ưu.', N'Làm việc cùng Tiến sĩ dữ liệu, phúc lợi VIP.', N'Python, Machine Learning', N'Senior', N'Full-time', 35000000, 55000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- Giao Hàng Nhanh (110)
(110, 1, N'C# Backend Developer (.NET Core)', N'Phát triển ứng dụng di động nội bộ của GHN.', N'C#, .NET Core, Web API, PostgreSQL.', N'Review lương hàng năm, cấp thiết bị làm việc tốt.', N'C#, .NET, PostgreSQL', N'Mid-Level', N'Full-time', 16000000, 26000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(110, 11, N'Manual QC Tester', N'Kiểm thử ứng dụng người giao hàng của GHN.', N'Viết testcase, API Testing với Postman, cẩn thận.', N'Được training nghiệp vụ Logistics chuyên nghiệp.', N'Manual Test, Postman', N'Junior', N'Full-time', 10000000, 14000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- KMS Technology (111)
(111, 4, N'Fullstack JS Developer (MERN)', N'Phát triển dự án Outsourcing cho thị trường Mỹ.', N'ReactJS, NodeJS, MongoDB, AWS cơ bản.', N'Lương net 100% trong thời gian thử việc, khám sức khỏe VIP.', N'ReactJS, Node.js, MongoDB', N'Mid-Level', N'Hybrid', 18000000, 32000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(111, 13, N'Automation QA Engineer (Playwright)', N'Xây dựng khung test tự động thế hệ mới.', N'Playwright, JavaScript/TypeScript, CI/CD.', N'Đóng bảo hiểm full lương, trợ cấp học ngoại ngữ.', N'Automation Test, TypeScript', N'Mid-Level', N'Full-time', 16000000, 26000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- DEK Technologies (112)
(112, 17, N'Embedded C++ Engineer', N'Lập trình phần mềm viễn thông 5G nhúng.', N'C++, Linux, Network Socket Programming.', N'Môi trường chuẩn Úc, chuyên nghiệp, cân bằng.', N'C++, Linux, Networking', N'Mid-Level', N'Full-time', 20000000, 35000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(112, 8, N'Linux System Engineer (Telco)', N'Cấu hình tối ưu hệ thống ảo hóa cho viễn thông.', N'Linux, Shell Scripting, Docker, OpenStack.', N'Trợ cấp đi onsite nước ngoài (Úc, Thuỵ Điển).', N'Linux, Docker, Shell', N'Mid-Level', N'Full-time', 18000000, 30000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- SmartOSC (113)
(113, 1, N'Magento Backend Developer', N'Phát triển ứng dụng thương mại Magento v2.', N'PHP, Magento 2, MySQL, JavaScript cơ bản.', N'Thưởng hiệu quả dự án hàng quý, cơ hội du lịch.', N'PHP, Magento, MySQL', N'Mid-Level', N'Full-time', 18000000, 32000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(113, 3, N'Frontend UI Developer (SASS/JS)', N'Xây dựng giao diện web chuẩn responsive.', N'HTML, SASS, JavaScript, Bootstrap/Tailwind.', N'Laptop xịn, lớp tiếng Anh giao tiếp tại văn phòng.', N'HTML, CSS, JavaScript', N'Junior', N'Full-time', 11000000, 16000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Rikkeisoft (114)
(114, 1, N'Senior Java Web Engineer', N'Thiết kế và phát triển dịch vụ web lớn cho đối tác Nhật.', N'Java, Spring Boot, Oracle Database, AWS.', N'Thưởng tiếng Nhật hàng tháng lên tới 5 triệu đồng.', N'Java, Spring Boot, SQL', N'Senior', N'Full-time', 25000000, 42000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(114, 5, N'Android Kotlin Developer', N'Phát triển ứng dụng ví di động Nhật Bản.', N'Kotlin, Coroutines, MVVM, Clean Architecture.', N'Cơ hội onsite lâu dài tại Tokyo, Nhật Bản.', N'Android, Kotlin, SQLite', N'Mid-Level', N'Full-time', 18000000, 30000000, 3, '2026-09-30', 'APPROVED', GETDATE()),

-- Luvina (115)
(115, 2, N'Junior C/C++ Developer', N'Đào tạo và phát triển ứng dụng hệ thống nhúng xe hơi.', N'C/C++, cấu trúc dữ liệu tốt, có tư duy logic.', N'Được đào tạo bài bản với giáo trình của hãng xe Nhật.', N'C, C++', N'Junior', N'Full-time', 10000000, 16000000, 8, '2026-09-30', 'APPROVED', GETDATE()),
(115, 3, N'Frontend VueJS Developer', N'Phát triển giao diện web bán hàng Nhật Bản.', N'Vue.js, JavaScript, CSS3, HTML5.', N'Đồng nghiệp thân thiện, sếp người Nhật tâm lý.', N'VueJS, JavaScript, CSS', N'Junior', N'Full-time', 11000000, 17000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Sun* Asterisk (116)
(116, 4, N'Fullstack Rails & ReactJS Engineer', N'Xây dựng các sản phẩm khởi nghiệp đầy sáng tạo.', N'Ruby on Rails, ReactJS, MySQL, Docker.', N'Được chọn thiết bị làm việc, giờ làm việc linh hoạt.', N'Ruby, ReactJS, Docker', N'Mid-Level', N'Hybrid', 20000000, 35000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(116, 19, N'Python AI/ML Specialist', N'Nghiên cứu triển khai mô hình học sâu nhận dạng giọng nói.', N'Python, PyTorch, Librosa, xử lý âm thanh.', N'Cơ hội nghiên cứu các công nghệ tương lai.', N'Python, PyTorch, AI', N'Senior', N'Full-time', 35000000, 60000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- VTI (117)
(117, 1, N'AWS Cloud Engineer (Solutions)', N'Tư vấn và thiết kế cơ sở hạ tầng đám mây cho khách hàng.', N'AWS Certified, EC2, RDS, VPC, Terraform.', N'Tài trợ 100% lệ phí thi chứng chỉ AWS.', N'AWS, Cloud, Terraform', N'Mid-Level', N'Full-time', 18000000, 30000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(117, 11, N'Automation QA Engineer (Robot)', N'Xây dựng kịch bản kiểm thử tự động với Robot Framework.', N'Python, Robot Framework, Selenium.', N'Phúc lợi y tế mở rộng, công ty hỗ trợ ăn trưa.', N'Automation, Robot, Python', N'Mid-Level', N'Full-time', 15000000, 23000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- SotaTek (118)
(118, 1, N'Blockchain Smart Contract Engineer', N'Viết các hợp đồng thông minh tối ưu bảo mật.', N'Solidity, Ethereum, Web3.js/Ethers.js.', N'Mức thu nhập hấp dẫn thuộc top thị trường blockchain.', N'Solidity, Web3, Blockchain', N'Mid-Level', N'Full-time', 25000000, 45000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(118, 3, N'ReactJS Web Developer', N'Phát triển ứng dụng Web3 Dashboard đẹp mắt.', N'ReactJS, Tailwind CSS, TypeScript.', N'Được tiếp cận công nghệ blockchain tiên tiến.', N'ReactJS, TypeScript, CSS', N'Mid-Level', N'Hybrid', 16000000, 27000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- NextPay (119)
(119, 1, N'Java backend Developer (Payment)', N'Nâng cấp hệ thống cổng thanh toán quốc gia.', N'Java Core, Spring Boot, Oracle Database.', N'Gói bảo hiểm toàn diện của tập đoàn, lương tháng 14.', N'Java, Spring Boot, SQL', N'Mid-Level', N'Full-time', 17000000, 28000000, 4, '2026-09-30', 'APPROVED', GETDATE()),
(119, 5, N'React Native App Engineer', N'Tối ưu hóa ứng dụng thanh toán NextPay trên di động.', N'React Native, JavaScript, Android/iOS native.', N'Được đào tạo nâng cao về Fintech.', N'React Native, Mobile', N'Mid-Level', N'Full-time', 16000000, 26000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- KiotViet (120)
(120, 1, N'ASP.NET Core Web Engineer', N'Xây dựng các phân hệ quản lý kho hàng KiotViet.', N'C#, ASP.NET Core, SQL Server, Redis.', N'Môi trường Product chuyên nghiệp, tôn trọng cá nhân.', N'C#, .NET Core, SQL Server', N'Mid-Level', N'Full-time', 18000000, 30000000, 5, '2026-09-30', 'APPROVED', GETDATE()),
(120, 11, N'QC Manual Test Specialist', N'Đảm bảo chất lượng các ứng dụng quản lý bán hàng.', N'Manual Testing, viết kịch bản test chi tiết.', N'Cấp laptop, đầy đủ phúc lợi, du lịch hè.', N'Manual Test, Testcase', N'Mid-Level', N'Full-time', 12000000, 18000000, 4, '2026-09-30', 'APPROVED', GETDATE()),

-- Sentifi (121)
(121, 18, N'Python Data Pipeline Engineer', N'Xây dựng hệ thống quét và phân tích tin tức tài chính.', N'Python, Scrapy/BeautifulSoup, Spark cơ bản.', N'Được tiếp cận công nghệ xử lý ngôn ngữ tự nhiên.', N'Python, ETL, Data', N'Mid-Level', N'Full-time', 18000000, 32000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(121, 19, N'Machine Learning Researcher (NLP)', N'Nghiên cứu mô hình phân tích cảm xúc từ mạng xã hội.', N'Python, PyTorch, HuggingFace, Transformer.', N'Mức đãi ngộ cực tốt dành cho nhân tài AI.', N'AI, NLP, PyTorch', N'Senior', N'Full-time', 30000000, 55000000, 1, '2026-09-30', 'APPROVED', GETDATE()),

-- Cốc Cốc (122)
(122, 1, N'C++ Browser Core Developer', N'Tối ưu hóa mã nguồn Chromium cho Cốc Cốc.', N'C++, Chromium Core, Win32 API, Linux.', N'Phúc lợi y tế mở rộng quốc tế, văn phòng hạng A.', N'C++, Chromium, Win32', N'Senior', N'Full-time', 30000000, 55000000, 2, '2026-09-30', 'APPROVED', GETDATE()),
(122, 3, N'Frontend Web Developer (Adtech)', N'Phát triển ứng dụng quản lý quảng cáo Cốc Cốc.', N'ReactJS, TypeScript, Webpack, CSS Grid.', N'Đánh giá hiệu suất tăng lương 2 lần một năm.', N'ReactJS, TypeScript, CSS', N'Mid-Level', N'Full-time', 18000000, 28000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Gear Inc (123)
(123, 2, N'NodeJS/Python Dev (Automation)', N'Xây dựng hệ thống tự động hóa kiểm duyệt hình ảnh.', N'NodeJS, Python, OpenCV/TensorFlow cơ bản.', N'Không OT, có phòng chơi game, ăn vặt miễn phí.', N'NodeJS, Python, AI', N'Mid-Level', N'Full-time', 16000000, 26000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(123, 10, N'IT Support Specialist', N'Quản trị vận hành mạng nội bộ và hỗ trợ thiết bị.', N'CCNA/MCSA cơ bản, xử lý sự cố tốt.', N'Được làm việc trong môi trường đa văn hoá.', N'IT Support, Network', N'Junior', N'Full-time', 10000000, 15000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Axon Active (124)
(124, 4, N'Fullstack Java/React Agile Engineer', N'Phát triển ứng dụng Web Enterprise cho đối tác Thuỵ Sĩ.', N'Java, Spring Boot, ReactJS, Scrum/Agile.', N'Bữa trưa miễn phí tại nhà ăn công ty, không OT.', N'Java, ReactJS, Scrum', N'Mid-Level', N'Full-time', 20000000, 32000000, 3, '2026-09-30', 'APPROVED', GETDATE()),
(124, 11, N'Agile Test Engineer (QA Manual)', N'Đảm bảo chất lượng phần mềm trong dự án Agile.', N'Có chứng chỉ ISTQB, giao tiếp tiếng Anh tốt.', N'Được tài trợ tham gia các sự kiện Agile quốc tế.', N'QA, Test, English', N'Mid-Level', N'Full-time', 15000000, 22000000, 2, '2026-09-30', 'APPROVED', GETDATE()),

-- Enclave (125)
(125, 1, N'Junior Java Web Developer', N'Tham gia phát triển dự án quản lý bệnh viện tại Mỹ.', N'Java Core, OOP vững, SQL, HTML/JS cơ bản.', N'Môi trường lành mạnh, hỗ trợ học tập tốt nhất.', N'Java, SQL, JavaScript', N'Junior', N'Full-time', 10000000, 16000000, 6, '2026-09-30', 'APPROVED', GETDATE()),
(125, 7, N'Flutter Cross-platform Developer', N'Phát triển ứng dụng thể thao thông minh đa nền tảng.', N'Flutter, Dart, Bloc Pattern, Git.', N'Bảo hiểm mở rộng toàn diện, du lịch hè VIP.', N'Flutter, Dart', N'Mid-Level', N'Full-time', 15000000, 24000000, 2, '2026-09-30', 'APPROVED', GETDATE());

-- =====================================================
-- 5. ÁNH XẠ JOBS VÀO BẢNG CHÂN TRỊ TỈNH THÀNH (job_post_province)
-- =====================================================
PRINT N'Đang ánh xạ tỉnh thành cho 50 jobs...';

DECLARE @ProvincesTable TABLE (
    Id INT IDENTITY(1, 1),
    ProvinceId INT
);

-- Danh sách 50 ID tỉnh thành không bị trùng lắp (tương ứng thứ tự 50 Jobs ở trên)
INSERT INTO @ProvincesTable (ProvinceId) VALUES
(1), (28), (2), (31), (3), (49), (4), (11), (5), (55),
(62), (8), (40), (56), (41), (57), (32), (14), (22), (7),
(39), (58), (36), (33), (17), (1), (2), (28), (3), (31),
(4), (49), (5), (11), (62), (55), (40), (8), (41), (56),
(32), (57), (22), (14), (39), (7), (36), (58), (17), (33);

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
    -- 6. THÊM TECH STACK MẪU CHO MỖI JOB (job_tech_stack)
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

PRINT N'Hoàn thành thêm dữ liệu thành công!';
GO
