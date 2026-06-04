USE ITRecruitmentDB;
GO

SET NOCOUNT ON;

-- Khai báo cấu trúc bảng tạm để lưu lại danh sách JobId tự động sinh ra
DECLARE @InsertedJobs TABLE (
    RowNumber INT IDENTITY(1,1),
    JobId INT,
    Status NVARCHAR(50)
);

-- =============================================
-- 1. INSERT 20 RECORDS CHO BẢNG job_post
-- =============================================
INSERT INTO dbo.job_post (
    recruiter_id, position_id, recruiter_package_history_id, 
    title, description, requirement, benefit, skill, 
    experience_level, location, working_model, salary_min, salary_max, 
    hiring_quota, deadline, status, priority_score, application_count, 
    approved_at, rejected_reason, created_at, moderator_id
)
OUTPUT inserted.job_id, inserted.status INTO @InsertedJobs(JobId, Status)
VALUES 
-- 1. PENDING (Chờ duyệt: Không có approved_at, không có rejected_reason)
(4, 1, 2, N'Senior .NET Core Developer', N'Phát triển hệ thống microservices lõi và tối ưu truy vấn dữ liệu lớn.', N'Tối thiểu 4 năm kinh nghiệm với C# .NET Core, SQL Server.', N'Lương tháng 13, bảo hiểm sức khỏe cao cấp độc quyền.', N'C#, .NET Core, SQL', N'Senior', N'Hà Nội', N'Full-time', 25000000, 40000000, 2, '2026-08-15', 'PENDING', 50, 0, NULL, NULL, '2026-06-03 09:00:00', NULL),

-- 2. APPROVED (Đang hoạt động: Đã duyệt sau khi tạo 4 tiếng)
(4, 3, 2, N'Frontend Angular Developer', N'Xây dựng giao diện Dashboard quản trị hệ thống phân tích dữ liệu.', N'Kinh nghiệm thực chiến Angular 14+, TypeScript, RxJS.', N'Thưởng dự án, cấp Macbook Pro, phụ cấp ăn trưa.', N'Angular, TypeScript, CSS', N'Mid-Level', N'Hà Nội', N'Hybrid', 18000000, 28000000, 3, '2026-07-20', 'APPROVED', 75, 4, '2026-06-01 14:30:00', NULL, '2026-06-01 10:30:00', 8),

-- 3. REJECTED (Bị từ chối: Có lý do từ chối, không có approved_at)
(4, 4, 2, N'Fullstack Web NodeJS/React', N'Tham gia xây dựng sản phẩm E-Commerce từ giai đoạn khởi tạo.', N'Thành thạo ReactJS và các framework NodeJS (Express, NestJS).', N'Review lương 2 lần/năm, lộ trình thăng tiến rõ ràng.', N'ReactJS, NodeJS, MongoDB', N'Junior/Mid', N'Hồ Chí Minh', N'Full-time', 15000000, 25000000, 5, '2026-06-30', 'REJECTED', 30, 0, NULL, N'Nội dung tin chứa liên kết ngoài hệ thống và thông tin cá nhân sai quy định.', '2026-06-02 08:00:00', 8),

-- 4. CLOSED (Đã đóng thủ công/hết hạn sớm: Có approved_at)
(4, 8, 2, N'DevOps AWS Engineer', N'Thiết lập, tối ưu và vận hành hệ thống CI/CD trên nền tảng AWS Cloud.', N'Có chứng chỉ AWS, kinh nghiệm sâu với Docker, Kubernetes, Terraform.', N'Làm việc 5 ngày/tuần, gói phúc lợi teambuilding hàng quý.', N'AWS, Docker, CI/CD', N'Senior', N'Hồ Chí Minh', N'Remote', 35000000, 55000000, 1, '2026-07-01', 'CLOSED', 80, 2, '2026-05-16 11:00:00', NULL, '2026-05-15 15:00:00', 8),

-- 5. CLOSED (Hết hạn: Deadline TRƯỚC 04/06/2026, có approved_at)
(4, 11, 2, N'Automation Tester Specialist', N'Viết test script tự động hóa kiểm thử cho các sản phẩm Web & Mobile.', N'Kinh nghiệm vững vàng với Selenium WebDriver, Java hoặc Python.', N'Lương tháng 14, hỗ trợ chi phí thi các chứng chỉ quốc tế.', N'Selenium, Java, Automation', N'Mid-Level', N'Hà Nội', N'Full-time', 16000000, 26000000, 2, '2026-05-25', 'CLOSED', 40, 12, '2026-04-02 09:15:00', NULL, '2026-04-01 11:00:00', 8),

-- 6. PENDING
(4, 5, 2, N'iOS Swift Engineer', N'Phát triển ứng dụng di động Native đáp ứng hàng triệu người dùng.', N'Hơn 2 năm làm việc chuyên sâu với Swift, UIKit và SwiftUI.', N'Phụ cấp gửi xe, trang thiết bị hiện đại, môi trường trẻ trung.', N'iOS, Swift, Xcode', N'Mid-Level', N'Hồ Chí Minh', N'Full-time', 20000000, 32000000, 2, '2026-07-15', 'PENDING', 60, 0, NULL, NULL, '2026-06-03 16:20:00', NULL),

-- 7. APPROVED
(4, 14, 2, N'IT Business Analyst (BA)', N'Khảo sát yêu cầu từ khách hàng doanh nghiệp, viết tài liệu SRS, User Story.', N'Kinh nghiệm BA trên 2 năm trong lĩnh vực Fintech hoặc ERP.', N'Cơ hội Onsite ngắn hạn, bảo hiểm sức khỏe toàn diện.', N'BA, SQL, UML, Jira', N'Mid-Level', N'Hà Nội', N'Full-time', 18000000, 30000000, 2, '2026-07-05', 'APPROVED', 70, 3, '2026-05-29 10:00:00', NULL, '2026-05-28 14:00:00', 8),

-- 8. APPROVED
(4, 2, 2, N'Junior Python Developer', N'Phát triển, bảo trì hệ thống cào dữ liệu lớn và các công cụ Automation.', N'Nắm chắc Python cơ bản, OOP, cấu trúc dữ liệu, thư viện BS4/Scrapy.', N'Được đào tạo bài bản bởi Mentor Senior giàu kinh nghiệm.', N'Python, BeautifulSoup, SQL', N'Junior', N'Hồ Chí Minh', N'Full-time', 11000000, 16000000, 4, '2026-06-25', 'APPROVED', 50, 15, '2026-05-26 09:00:00', NULL, '2026-05-25 17:30:00', 8),

-- 9. REJECTED
(4, 18, 2, N'Data Engineer (Data Pipeline)', N'Xây dựng luồng xử lý và làm sạch dữ liệu lớn phục vụ hệ thống BI.', N'Kinh nghiệm xây dựng kiến trúc dữ liệu vững với Spark, Hadoop, SQL.', N'Môi trường năng động, review hiệu suất công việc rõ ràng.', N'Spark, Python, BigData', N'Senior', N'Hà Nội', N'Hybrid', 30000000, 50000000, 1, '2026-06-30', 'REJECTED', 85, 0, NULL, N'Thông tin mức lương không rõ ràng, yêu cầu kỹ năng quá chung chung.', '2026-05-20 10:00:00', 8),

-- 10. APPROVED
(4, 12, 2, N'Manual QC Tester', N'Lập kế hoạch test case, thực hiện test thủ công, quản lý theo dõi bug.', N'Hiểu quy trình phát hành phần mềm, cẩn thận, chịu áp lực tốt.', N'Thưởng các ngày lễ tết, cấp máy tính làm việc tại văn phòng.', N'Manual Test, Testcase, Jira', N'Junior/Mid', N'Hồ Chí Minh', N'Full-time', 10000000, 15000000, 3, '2026-05-10', 'APPROVED', 30, 9, '2026-05-11 13:00:00', NULL, '2026-06-10 15:45:00', 8),

-- 11. PENDING
(4, 1, 2, N'Golang Backend Engineer', N'Xây dựng hệ thống Core thanh toán thời gian thực chịu tải cực lớn.', N'Kinh nghiệm lập trình Golang tốt hoặc vững ngôn ngữ hướng đối tượng.', N'Môi trường đa quốc gia, làm việc hoàn toàn bằng tiếng Anh.', N'Golang, Redis, Kafka', N'Mid-Level', N'Hà Nội', N'Full-time', 22000000, 38000000, 2, '2026-07-18', 'PENDING', 65, 0, NULL, NULL, '2026-06-03 11:10:00', NULL),

-- 12. APPROVED
(4, 7, 2, N'Flutter Mobile Developer', N'Phát triển ứng dụng mạng xã hội chạy mượt mà đa nền tảng.', N'Hơn 1.5 năm kinh nghiệm làm sản phẩm thực tế với Flutter & Dart.', N'Hỗ trợ cơm trưa, miễn phí đồ ăn nhẹ tại pantry công ty.', N'Flutter, Dart, Bloc', N'Mid-Level', N'Hồ Chí Minh', N'Hybrid', 16000000, 24000000, 2, '2026-07-28', 'APPROVED', 60, 5, '2026-05-21 16:00:00', NULL, '2026-05-20 13:20:00', 8),

-- 13. APPROVED
(4, 3, 2, N'Senior ReactJS Developer', N'Làm chủ kiến trúc giao diện các module lõi trực thuộc hệ sinh thái.', N'Tối thiểu 4 năm kinh nghiệm Frontend, sâu sắc về NextJS, Redux Toolkit.', N'Mức lương đột phá, tháng lương thứ 14 ổn định hằng năm.', N'ReactJS, NextJS, Redux', N'Senior', N'Hồ Chí Minh', N'Full-time', 28000000, 45000000, 1, '2026-07-12', 'APPROVED', 90, 8, '2026-05-18 10:15:00', NULL, '2026-05-17 11:00:00', 8),

-- 14. REJECTED
(4, 19, 2, N'AI / Machine Learning Engineer', N'Nghiên cứu và tích hợp các mô hình Generative AI tối ưu hóa sản phẩm.', N'Nền tảng toán tối ưu vững, kinh nghiệm sâu với PyTorch, TensorFlow.', N'Làm việc cùng chuyên gia đầu ngành, lộ trình R&D rõ ràng.', N'AI, Python, PyTorch', N'Mid/Senior', N'Hà Nội', N'Full-time', 35000000, 60000000, 2, '2026-07-01', 'REJECTED', 95, 0, NULL, N'Tên vị trí và mô tả công việc bằng ngôn từ không chuẩn mực lịch sự.', '2026-05-25 14:00:00', 8),

-- 15. CLOSED
(4, 13, 2, N'Embedded Systems Engineer', N'Lập trình firmware nhúng, điều khiển vi xử lý cho thiết bị IoT.', N'Thành thạo C/C++, hiểu biết sâu sắc về kiến trúc phần cứng ARM.', N'Trợ cấp độc hại phòng lab, du lịch nghỉ dưỡng cao cấp.', N'Embedded C, C++, IoT, ARM', N'Mid-Level', N'Hà Nội', N'Full-time', 20000000, 32000000, 1, '2026-06-30', 'CLOSED', 55, 1, '2026-05-06 09:30:00', NULL, '2026-05-05 10:15:00', 8),

-- 16. CLOSED
(4, 16, 2, N'Technical Project Manager (IT PM)', N'Quản lý tiến độ dự án, điều phối nhân sự, làm việc chặt chẽ với khách hàng.', N'Ít nhất 2 năm làm PM, giao tiếp tiếng Anh trôi chảy bắt buộc.', N'Thưởng quản lý, cổ phần thưởng hấp dẫn tùy hiệu suất.', N'PM, Agile, Scrum', N'Manager', N'Hồ Chí Minh', N'Full-time', 35000000, 50000000, 1, '2026-05-01', 'CLOSED', 80, 14, '2026-03-16 11:00:00', NULL, '2026-03-15 14:30:00', 8),

-- 17. PENDING
(4, 3, 2, N'VueJS Frontend Developer', N'Nâng cấp hệ thống Web App SaaS sang kiến trúc Single Page.', N'Thành thạo VueJS v3 (Composition API), Pinia, Tailwind CSS.', N'Môi trường ít OT, cân bằng tốt giữa công việc và đời sống.', N'VueJS, Pinia, CSS', N'Junior/Mid', N'Hồ Chí Minh', N'Full-time', 13000000, 20000000, 2, '2026-07-25', 'PENDING', 45, 0, NULL, NULL, '2026-06-03 15:45:00', NULL),

-- 18. APPROVED
(4, 1, 2, N'Java Spring Boot Backend Engineer', N'Tái cấu trúc hệ thống Monolith cũ sang mô hình kiến trúc Microservices.', N'Tối thiểu 3 năm kinh nghiệm với Java Core, Spring Boot, Hibernate.', N'Phụ cấp gym/yoga, trà cà phê miễn phí không giới hạn.', N'Java, Spring Boot, MySQL', N'Mid-Level', N'Đà Nẵng', N'Full-time', 17000000, 27000000, 3, '2026-07-10', 'APPROVED', 65, 2, '2026-05-22 09:00:00', NULL, '2026-05-21 15:00:00', 8),

-- 19. APPROVED
(4, 9, 2, N'System Linux Administrator', N'Giám sát hoạt động, bảo mật và sao lưu dữ liệu cụm máy chủ Linux.', N'Kinh nghiệm quản trị Ubuntu/CentOS, am hiểu Bash Script và Network.', N'Tham gia trực trực ca nhận phụ cấp hấp dẫn, quà tặng sinh nhật.', N'Linux, Bash Script, Network', N'Mid-Level', N'Hà Nội', N'Full-time', 15000000, 23000000, 2, '2026-07-02', 'APPROVED', 50, 4, '2026-05-26 10:30:00', NULL, '2026-05-25 09:30:00', 8),

-- 20. CLOSED
(4, 4, 2, N'PHP Laravel Web Developer', N'Bảo trì, phát triển mở rộng các tính năng mới cho cổng thông tin.', N'Có kinh nghiệm lập trình PHP tốt và tối thiểu 1 năm làm với Laravel.', N'Đồng nghiệp thân thiện, sếp tâm lý, nhiều hoạt động nội bộ.', N'PHP, Laravel, MySQL', N'Junior', N'Hà Nội', N'Full-time', 10000000, 16000000, 2, '2026-05-15', 'CLOSED', 35, 11, '2026-04-11 15:00:00', NULL, '2026-04-10 16:30:00', 8);


-- =============================================
-- 2. INSERT MAPPING SANG BẢNG job_technology
-- (Mỗi Job có ngẫu nhiên 2 đến 3 Tech stacks duy nhất)
-- =============================================
DECLARE @CurrentRow INT = 1;
DECLARE @TotalInserted INT = (SELECT COUNT(*) FROM @InsertedJobs);
DECLARE @TargetJobId INT;

WHILE @CurrentRow <= @TotalInserted
BEGIN
    SELECT @TargetJobId = JobId FROM @InsertedJobs WHERE RowNumber = @CurrentRow;

    -- Lựa chọn kịch bản chèn Tech ngẫu nhiên xen kẽ để tránh trùng lặp cặp Khóa chính (job_id, tech_id)
    IF @CurrentRow % 4 = 0
    BEGIN
        INSERT INTO dbo.job_tech_stack(job_id, tech_id) VALUES (@TargetJobId, 1), (@TargetJobId, 2), (@TargetJobId, 3); -- .NET / C# / SQL
    END
    ELSE IF @CurrentRow % 4 = 1
    BEGIN
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 4), (@TargetJobId, 5); -- Java / Spring Boot
    END
    ELSE IF @CurrentRow % 4 = 2
    BEGIN
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 6), (@TargetJobId, 7), (@TargetJobId, 8); -- Python / Django / React
    END
    ELSE
    BEGIN
        INSERT INTO dbo.job_tech_stack (job_id, tech_id) VALUES (@TargetJobId, 9), (@TargetJobId, 10); -- NodeJS / Angular
    END

    SET @CurrentRow = @CurrentRow + 1;
END;
GO

USE ITRecruitmentDB;
GO

SET NOCOUNT ON;

-- Khai báo cấu trúc bảng tạm để lưu lại danh sách JobId tự động sinh ra
DECLARE @NewInsertedJobs TABLE (
    RowNumber INT IDENTITY(1,1),
    JobId INT
);

-- =============================================
-- 1. INSERT 10 JOBPOSTS TRẠNG THÁI APPROVED
-- =============================================
INSERT INTO dbo.job_post (
    recruiter_id, position_id, recruiter_package_history_id,
    title, description, requirement, benefit, skill,
    experience_level, location, working_model, salary_min, salary_max,
    hiring_quota, deadline, status, priority_score, application_count,
    approved_at, rejected_reason, created_at, moderator_id
)
OUTPUT inserted.job_id INTO @NewInsertedJobs(JobId)
VALUES
-- 1: Intern + Fulltime Onsite
(5, 2, 2, N'Thực tập sinh Lập trình Python (Intern)', N'Tham gia hỗ trợ đội ngũ phát triển xây dựng các công cụ thu thập và xử lý số liệu.', N'Sinh viên năm 3-4 chuyên ngành CNTT, nắm chắc kiến thức OOP căn bản.', N'Hỗ trợ phụ cấp thực tập, có cơ hội lên chính thức sau 3 tháng.', N'Python, OOP', 
N'Intern', N'Hà Nội', N'Fulltime Onsite', 3000000, 6000000, 5, '2026-08-30', 'APPROVED', 40, 2, '2026-06-02 10:00:00', NULL, '2026-06-01 15:30:00', 8),

-- 2: Fresher + Hybrid
(5, 3, 2, N'Fresher ReactJS Developer', N'Được đào tạo và tham gia trực tiếp vào dự án Web App của đối tác Singapore.', N'Đã có đồ án nền tảng HTML/CSS/JS tốt, biết cơ bản về React hooks.', N'Review lương sau thời gian thử việc, hỗ trợ thiết bị làm việc.', N'ReactJS, JavaScript', 
N'Fresher', N'Hồ Chí Minh', N'Hybrid', 8000000, 12000000, 3, '2026-07-25', 'APPROVED', 45, 7, '2026-06-03 09:15:00', NULL, '2026-06-02 11:00:00', 8),

-- 3: Junior + Fulltime Remote
(5, 11, 2, N'Junior QA Manual Tester', N'Thực hiện viết testcase và thực thi kiểm thử các chức năng hệ thống thương mại điện tử.', N'Có từ 1 năm kinh nghiệm test web/app, hiểu biết về quy trình Agile/Scrum.', N'Làm việc Remote tự do, cung cấp tài khoản học tập Udemy Business.', N'Manual Test, Testcase', 
N'Junior', N'Đà Nẵng', N'Fulltime Remote', 11000000, 16000000, 2, '2026-07-15', 'APPROVED', 50, 4, '2026-05-30 14:00:00', NULL, '2026-05-29 14:30:00', 8),

-- 4: Middle + Parttime
(5, 1, 2, N'C# .NET Core Developer (Part-time)', N'Hỗ trợ bảo trì, nâng cấp một số module cũ thuộc hệ thống quản trị nội bộ.', N'Tối thiểu 2 năm kinh nghiệm làm việc với .NET MVC / .NET Core, SQL Server.', N'Thời gian làm việc linh hoạt, trả lương theo giờ hoặc theo gói task hoàn thành.', N'C#, .NET Core', 
N'Middle', N'Hà Nội', N'Parttime', 10000000, 15000000, 1, '2026-06-28', 'APPROVED', 55, 1, '2026-06-01 16:00:00', NULL, '2026-05-31 16:30:00', 8),

-- 5: Senior + Freelance
(5, 4, 2, N'Fullstack Node/Vue Expert (Freelance)', N'Chịu trách nhiệm kiến trúc lại phần API Gateway và tối ưu hóa UI/UX ứng dụng.', N'Trực chiến trên 4 năm Fullstack, có sản phẩm thực tế chứng minh năng lực.', N'Thù lao dự án cực kỳ hấp dẫn, làm việc độc lập không gò bó.', N'NodeJS, VueJS', 
N'Senior', N'Toàn quốc', N'Freelance', 30000000, 50000000, 2, '2026-07-10', 'APPROVED', 70, 3, '2026-05-28 11:20:00', NULL, '2026-05-27 13:00:00', 8),

-- 6: Lead / Manager + Fulltime Onsite
(5, 16, 2, N'Project Manager (IT PM)', N'Quản lý vòng đời dự án, làm việc trực tiếp với khách hàng Nhật Bản để chốt spec.', N'Tối thiểu 5 năm kinh nghiệm phần mềm, giao tiếp tiếng Nhật từ N2 trở lên.', N'Thưởng quản lý, gói chăm sóc sức khỏe VIP cho cả gia đình.', N'Project Management, Agile', 
N'Lead', N'Hà Nội', N'Fulltime Onsite', 40000000, 65000000, 1, '2026-07-20', 'APPROVED', 90, 5, '2026-05-25 09:00:00', NULL, '2026-05-24 10:00:00', 8),

-- 7: Middle + Fulltime Onsite
(5, 8, 2, N'DevOps Cloud Engineer', N'Triển khai hạ tầng hạ tầng viễn thông sử dụng Docker, K8s trên nền tảng GCP.', N'Có kinh nghiệm build CI/CD pipelines, am hiểu sâu hệ điều hành Linux.', N'Lương net cạnh tranh, định hướng phát triển rõ ràng lên Architect.', N'Docker, K8s, CI/CD', 
N'Middle', N'Hồ Chí Minh', N'Fulltime Onsite', 22000000, 35000000, 2, '2026-08-01', 'APPROVED', 75, 2, '2026-06-02 15:30:00', NULL, '2026-06-02 09:00:00', 8),

-- 8: Junior + Hybrid
(5, 7, 2, N'Flutter Mobile Developer', N'Xây dựng các module giao diện người dùng cho ứng dụng đặt đồ ăn trực tuyến.', N'Có trên 1 năm kinh nghiệm lập trình ứng dụng di động Flutter/Dart.', N'Môi trường công nghệ hiện đại, tuần làm việc 2 ngày remote.', N'Flutter, Dart', 
N'Junior', N'Hồ Chí Minh', N'Hybrid', 14000000, 20000000, 2, '2026-07-18', 'APPROVED', 60, 9, '2026-06-01 10:00:00', NULL, '2026-05-31 15:00:00', 8),

-- 9: Senior + Fulltime Remote
(5, 18, 2, N'Senior Data Engineer', N'Xây dựng, bảo trì hạ tầng Data Warehouse, luồng xử lý ETL dữ liệu tài chính.', N'Hơn 3 năm kinh nghiệm với Big Data, am hiểu sâu Hadoop, Spark, Kafka.', N'Làm việc từ xa 100%, trợ cấp chi phí mua sắm ghế công thái học.', N'Hadoop, Spark, ETL', 
N'Senior', N'Toàn quốc', N'Fulltime Remote', 35000000, 55000000, 1, '2026-07-30', 'APPROVED', 85, 4, '2026-05-20 16:45:00', NULL, '2026-05-20 09:30:00', 8),

-- 10: Fresher + Parttime
(5, 14, 2, N'Trợ lý Business Analyst (Part-time BA)', N'Tham gia vẽ biểu đồ UseCase, viết tài liệu đặc tả hệ thống cùng Senior BA.', N'Tư duy logic tốt, biết sử dụng công cụ Figma, Miro hoặc Visio.', N'Cơ hội học hỏi quy trình chuẩn chỉnh, phù hợp làm thêm tích lũy kinh nghiệm.', N'BA, UML, Figma', 
N'Fresher', N'Hà Nội', N'Parttime', 5000000, 8000000, 2, '2026-07-05', 'APPROVED', 45, 12, '2026-05-29 11:00:00', NULL, '2026-05-28 14:00:00', 8);


-- =============================================
-- 2. INSERT MAPPING SANG BẢNG job_tech_stack
-- =============================================
DECLARE @CurrentRow INT = 1;
DECLARE @TotalInserted INT = (SELECT COUNT(*) FROM @NewInsertedJobs);
DECLARE @TargetJobId INT;

WHILE @CurrentRow <= @TotalInserted
BEGIN
    SELECT @TargetJobId = JobId FROM @NewInsertedJobs WHERE RowNumber = @CurrentRow;

    -- Phân bổ tech_id ngẫu nhiên xoay vòng để tạo dữ liệu đa dạng sinh động
    IF @CurrentRow % 3 = 0
    BEGIN
        INSERT INTO dbo.job_tech_stack(job_id, tech_id) VALUES (@TargetJobId, 1), (@TargetJobId, 2); -- C# / .NET
    END
    ELSE IF @CurrentRow % 3 = 1
    BEGIN
        INSERT INTO dbo.job_tech_stack(job_id, tech_id) VALUES (@TargetJobId, 6), (@TargetJobId, 8), (@TargetJobId, 9); -- Python / React / NodeJS
    END
    ELSE
    BEGIN
        INSERT INTO dbo.job_tech_stack(job_id, tech_id) VALUES (@TargetJobId, 4), (@TargetJobId, 10); -- Java / Angular
    END

    SET @CurrentRow = @CurrentRow + 1;
END;
GO

-- =============================================
-- 3. TRUY VẤN KIỂM TRA LẠI KẾT QUẢ
-- =============================================
SELECT j.job_id, j.title, j.experience_level, j.working_model, j.status, j.deadline,
       (SELECT COUNT(*) FROM dbo.job_tech_stack jt WHERE jt.job_id = j.job_id) AS TechStackCount
FROM dbo.job_post j
WHERE j.recruiter_id = 4 AND j.status = 'APPROVED'
ORDER BY j.job_id DESC;
GO
