-- =============================================
-- SAMPLE DATA FOR OPTIMIZED DATABASE
-- =============================================
USE DevHub;
GO
SET NOCOUNT ON;
-- =============================================
-- 1. INSERT MASTER DATA 
-- =============================================
-- 1.1 Danh mục Vị trí công việc (common_job_position)
SET IDENTITY_INSERT dbo.common_job_position ON;
INSERT INTO dbo.common_job_position (position_id, position_name, is_active)
VALUES (1, N'Senior Backend Developer', 1),
    (2, N'Junior Backend Developer', 1),
    (3, N'Frontend Developer', 1),
    (4, N'Fullstack Developer', 1),
    (5, N'Mobile Developer', 1),
    (6, N'React Native Developer', 1),
    (7, N'Flutter Developer', 1),
    (8, N'DevOps Engineer', 1),
    (9, N'System Administrator', 1),
    (10, N'IT Support', 1),
    (11, N'QA Engineer', 1),
    (12, N'Manual Tester', 1),
    (13, N'Automation Tester', 1),
    (14, N'Business Analyst', 1),
    (15, N'Product Manager', 1),
    (16, N'Project Manager', 1),
    (17, N'Embedded Engineer', 1),
    (18, N'Data Engineer', 1),
    (19, N'Machine Learning Engineer', 1),
    (20, N'UI/UX Designer', 1);
SET IDENTITY_INSERT dbo.common_job_position OFF;
-- 1.2 Danh mục Công nghệ (common_technology)
SET IDENTITY_INSERT dbo.common_technology ON;
INSERT INTO dbo.common_technology (tech_id, tech_name, category, is_active)
VALUES (1, N'Java', 'Backend', 1),
    (2, N'Spring Boot', 'Backend', 1),
    (3, N'.NET', 'Backend', 1),
    (4, N'C#', 'Backend', 1),
    (5, N'Python', 'Backend', 1),
    (6, N'Node.js', 'Backend', 1),
    (7, N'ReactJS', 'Frontend', 1),
    (8, N'Vue.js', 'Frontend', 1),
    (9, N'Angular', 'Frontend', 1),
    (10, N'TypeScript', 'Frontend', 1),
    (11, N'Flutter', 'Mobile', 1),
    (12, N'React Native', 'Mobile', 1),
    (13, N'Docker', 'DevOps', 1),
    (14, N'Kubernetes', 'DevOps', 1),
    (15, N'AWS', 'Cloud', 1),
    (16, N'Azure', 'Cloud', 1),
    (17, N'SQL Server', 'Database', 1),
    (18, N'PostgreSQL', 'Database', 1),
    (19, N'MySQL', 'Database', 1),
    (20, N'MongoDB', 'Database', 1),
    (21, N'Embedded C', 'Embedded', 1),
    (22, N'RTOS', 'Embedded', 1),
    (23, N'ARM Cortex', 'Embedded', 1),
    (24, N'Windows Server', 'System', 1),
    (25, N'Network Troubleshooting', 'System', 1),
    (26, N'Linux', 'System', 1),
    (27, N'Manual Testing', 'QA', 1),
    (28, N'Automation Testing', 'QA', 1),
    (29, N'Selenium', 'QA', 1),
    (30, N'SQL', 'Database', 1),
    (31, N'Power BI', 'Data', 1),
    (32, N'Business Analyst', 'BA', 1),
    (33, N'Cyber Security', 'Security', 1),
    (34, N'OOP', 'Basic', 1);
SET IDENTITY_INSERT dbo.common_technology OFF;
-- 1.3 Danh mục Tỉnh thành (province)
;
WITH seed(province_name) AS (
    SELECT v
    FROM (
            VALUES (N'Hà Nội'),
                (N'Hồ Chí Minh'),
                (N'Hải Phòng'),
                (N'Đà Nẵng'),
                (N'Cần Thơ'),
                (N'An Giang'),
                (N'Bà Rịa - Vũng Tàu'),
                (N'Bắc Giang'),
                (N'Bắc Kạn'),
                (N'Bạc Liêu'),
                (N'Bắc Ninh'),
                (N'Bến Tre'),
                (N'Bình Định'),
                (N'Bình Dương'),
                (N'Bình Phước'),
                (N'Bình Thuận'),
                (N'Cà Mau'),
                (N'Cao Bằng'),
                (N'Đắk Lắk'),
                (N'Đắk Nông'),
                (N'Điện Biên'),
                (N'Đồng Nai'),
                (N'Đồng Tháp'),
                (N'Gia Lai'),
                (N'Hà Giang'),
                (N'Hà Nam'),
                (N'Hà Tĩnh'),
                (N'Hải Dương'),
                (N'Hậu Giang'),
                (N'Hòa Bình'),
                (N'Hưng Yên'),
                (N'Khánh Hòa'),
                (N'Kiên Giang'),
                (N'Kon Tum'),
                (N'Lai Châu'),
                (N'Lâm Đồng'),
                (N'Lạng Sơn'),
                (N'Lào Cai'),
                (N'Long An'),
                (N'Nam Định'),
                (N'Nghệ An'),
                (N'Ninh Bình'),
                (N'Ninh Thuận'),
                (N'Phú Thọ'),
                (N'Phú Yên'),
                (N'Quảng Bình'),
                (N'Quảng Nam'),
                (N'Quảng Ngãi'),
                (N'Quảng Ninh'),
                (N'Quảng Trị'),
                (N'Sóc Trăng'),
                (N'Sơn La'),
                (N'Tây Ninh'),
                (N'Thái Bình'),
                (N'Thái Nguyên'),
                (N'Thanh Hóa'),
                (N'Thừa Thiên Huế'),
                (N'Tiền Giang'),
                (N'Trà Vinh'),
                (N'Tuyên Quang'),
                (N'Vĩnh Long'),
                (N'Vĩnh Phúc'),
                (N'Yên Bái')
        ) AS t(v)
)
INSERT INTO dbo.province (province_name)
SELECT s.province_name
FROM seed s;
-- =============================================
-- 2. USER ACCOUNT (Sử dụng Filtered Index cho Google_id NULL)
-- =============================================
SET IDENTITY_INSERT user_account ON;
INSERT INTO user_account (
        user_id,
        google_id,
        email,
        password_hash,
        user_type,
        is_active,
        created_at,
        last_login
    )
VALUES (
        1,
        NULL,
        'tuanpanh03@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        2,
        NULL,
        'tranthib@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        3,
        NULL,
        'levanc@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        4,
        NULL,
        'hr@fpt.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        5,
        NULL,
        'recruit@viettel.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        6,
        NULL,
        'hr@techcombank.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        7,
        NULL,
        'admin@itrecruitment.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'ADMIN',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        8,
        NULL,
        'mod@itrecruitment.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'MODERATOR',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        9,
        NULL,
        'phamvand@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        10,
        NULL,
        'hoangthie@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        11,
        NULL,
        'nguyenthif@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        12,
        NULL,
        'dovanha@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        13,
        NULL,
        'buitrungk@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        14,
        NULL,
        'vuthikim@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        15,
        NULL,
        'leminhhoang@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        16,
        NULL,
        'trankhact@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        17,
        NULL,
        'dangquochuy@gmail.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'CANDIDATE',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        18,
        NULL,
        'hr@vng.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        19,
        NULL,
        'talent@vingroup.net',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        20,
        NULL,
        'hr@nashtechglobal.com',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        21,
        NULL,
        'recruit@cmc.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    ),
    (
        22,
        NULL,
        'hr@bosch.com.vn',
        '$2a$11$XdhNrPCFlL.O6cm8/RPs7OZxQ8IEyAUdAWScJTQGWVQ0auQwNjVja',
        'RECRUITER',
        1,
        GETDATE(),
        GETDATE()
    );
SET IDENTITY_INSERT user_account OFF;
-- =============================================
-- 3. CANDIDATE DETAILS
-- =============================================
INSERT INTO dbo.candidate (
        candidate_id,
        full_name,
        gender,
        birthdate,
        phone,
        address,
        expected_salary_min,
        expected_salary_max,
        preferred_location,
        experience_years,
        profile_completion,
        cv_searchable
    )
VALUES (
        1,
        N'Phan Tuấn Anh',
        N'Nam',
        '2005-05-12',
        '0987654321',
        N'Hà Nội',
        12000000,
        18000000,
        N'Hà Nội',
        3,
        85,
        1
    ),
    (
        2,
        N'Trần Thị Bích',
        N'Nữ',
        '1999-08-20',
        '0978123456',
        N'Hồ Chí Minh',
        10000000,
        16000000,
        N'Hồ Chí Minh',
        2,
        75,
        1
    ),
    (
        3,
        N'Lê Văn Cường',
        N'Nam',
        '1997-03-15',
        '0967123456',
        N'Đà Nẵng',
        15000000,
        22000000,
        N'Đà Nẵng',
        5,
        90,
        1
    ),
    (
        9,
        N'Phạm Văn Đức',
        N'Nam',
        '1998-11-05',
        '0934567890',
        N'Hà Nội',
        15000000,
        23000000,
        N'Hà Nội',
        4,
        80,
        1
    ),
    (
        10,
        N'Hoàng Thị Lan',
        N'Nữ',
        '2000-02-14',
        '0945678901',
        N'Hồ Chí Minh',
        9000000,
        14000000,
        N'Hồ Chí Minh',
        1,
        70,
        1
    ),
    (
        11,
        N'Nguyễn Thị Thu',
        N'Nữ',
        '1998-07-22',
        '0956789012',
        N'Đà Nẵng',
        11000000,
        17000000,
        N'Đà Nẵng',
        3,
        78,
        1
    ),
    (
        12,
        N'Đỗ Văn Hải',
        N'Nam',
        '2001-09-10',
        '0967890123',
        N'Hà Nội',
        18000000,
        26000000,
        N'Hà Nội',
        6,
        92,
        1
    ),
    (
        13,
        N'Bùi Trung Kiên',
        N'Nam',
        '2000-04-18',
        '0978901234',
        N'Hồ Chí Minh',
        13000000,
        20000000,
        N'Hồ Chí Minh',
        2,
        65,
        1
    ),
    (
        14,
        N'Vũ Thị Kim Anh',
        N'Nữ',
        '1999-12-30',
        '0989012345',
        N'Hà Nội',
        12000000,
        19000000,
        N'Hà Nội',
        3,
        82,
        1
    ),
    (
        15,
        N'Lê Minh Hoàng',
        N'Nam',
        '1998-06-25',
        '0990123456',
        N'Đà Nẵng',
        14000000,
        21000000,
        N'Đà Nẵng',
        4,
        88,
        1
    ),
    (
        16,
        N'Trần Khắc Tùng',
        N'Nam',
        '1997-01-08',
        '0911234567',
        N'Hồ Chí Minh',
        16000000,
        24000000,
        N'Hồ Chí Minh',
        5,
        85,
        1
    ),
    (
        17,
        N'Đặng Quốc Huy',
        N'Nam',
        '1999-10-15',
        '0922345678',
        N'Hà Nội',
        10000000,
        15500000,
        N'Hà Nội',
        2,
        72,
        1
    );
-- =============================================
-- 4. COMPANY & RECRUITER DETAILS
-- =============================================
SET IDENTITY_INSERT dbo.company ON;
INSERT INTO dbo.company (
        company_id,
        company_name,
        company_address,
        company_description,
        website,
        industry,
        tax_code,
        average_rating,
        total_spent,
        is_verified,
        profile_completion
    )
VALUES (
        4,
        N'FPT Software',
        N'Hà Nội',
        N'FPT Software là công ty công nghệ hàng đầu Việt Nam',
        'https://fptsoftware.com',
        N'Phần mềm',
        '0101234567',
        4.1,
        45000000,
        1,
        75
    ),
    (
        5,
        N'Viettel Group',
        N'Hà Nội',
        N'Tập đoàn Công nghiệp - Viễn thông Quân đội',
        'https://viettel.com.vn',
        N'Telecom & IT',
        '0107654321',
        4.3,
        120000000,
        1,
        75
    ),
    (
        6,
        N'Techcombank',
        N'Hồ Chí Minh',
        N'Ngân hàng TMCP Kỹ Thương Việt Nam',
        'https://techcombank.com',
        N'Ngân hàng',
        '0109876543',
        3.8,
        35000000,
        0,
        75
    ),
    (
        18,
        N'VNG Corporation',
        N'Hồ Chí Minh',
        N'Công ty công nghệ hàng đầu với Zalo, ZaloPay...',
        'https://vng.com.vn',
        N'Internet & Game',
        '0101112223',
        4.2,
        250000000,
        1,
        75
    ),
    (
        19,
        N'VinGroup',
        N'Hà Nội',
        N'Tập đoàn công nghệ - ô tô - bất động sản',
        'https://vingroup.net',
        N'Multi-industry',
        '0102223334',
        4.5,
        180000000,
        1,
        75
    ),
    (
        20,
        N'NashTech',
        N'Hồ Chí Minh',
        N'Công ty outsourcing phần mềm toàn cầu',
        'https://nashtechglobal.com',
        N'Phần mềm',
        '0103334445',
        3.7,
        95000000,
        1,
        75
    ),
    (
        21,
        N'CMC Corporation',
        N'Hà Nội',
        N'Tập đoàn công nghệ CMC',
        'https://cmc.com.vn',
        N'Phần mềm & Dịch vụ IT',
        '0104445556',
        3.7,
        120000000,
        0,
        75
    ),
    (
        22,
        N'Bosch Vietnam',
        N'Hồ Chí Minh',
        N'Công ty công nghệ Đức tại Việt Nam',
        'https://bosch.com.vn',
        N'Ô tô & Công nghệ',
        '0105556667',
        4.0,
        80000000,
        1,
        75
    );
SET IDENTITY_INSERT dbo.company OFF;
INSERT INTO dbo.recruiter (
        recruiter_id,
        full_name,
        position,
        phone,
        company_id,
        is_company_admin
    )
VALUES (
        4,
        N'Phạm Thị Hương',
        N'HR Manager',
        '0901234567',
        4,
        1
    ),
    (
        5,
        N'Trần Minh Quân',
        N'Talent Acquisition Lead',
        '0912345678',
        5,
        1
    ),
    (
        6,
        N'Lê Thị Ngọc Anh',
        N'HR Specialist',
        '0923456789',
        6,
        1
    ),
    (
        18,
        N'Nguyễn Thị Mai',
        N'HR Director',
        '0933456789',
        18,
        1
    ),
    (
        19,
        N'Trần Hoàng Nam',
        N'Talent Manager',
        '0944567890',
        19,
        1
    ),
    (
        20,
        N'Lê Thị Thanh',
        N'HR Manager',
        '0955678901',
        20,
        1
    ),
    (
        21,
        N'Phạm Minh Châu',
        N'Recruitment Lead',
        '0966789012',
        21,
        1
    ),
    (
        22,
        N'Đỗ Thị Huyền',
        N'HR Specialist',
        '0977890123',
        22,
        1
    );
-- =============================================
-- 5. ADMIN & MODERATOR DETAILS
-- =============================================
INSERT INTO dbo.admin (admin_id, username, full_name, role)
VALUES (
        7,
        'admin_system',
        N'Nguyễn Văn Trưởng',
        'ADMIN'
    ),
    (
        8,
        'moderator_01',
        N'Trần Trung Thực',
        'MODERATOR'
    );
-- =============================================
-- 6. CANDIDATE SKILL
-- =============================================
INSERT INTO dbo.candidate_skill (candidate_id, tech_id, level)
VALUES (1, 1, 'Advanced'),
    (1, 2, 'Advanced'),
    (1, 13, 'Intermediate'),
    (2, 7, 'Advanced'),
    (2, 10, 'Advanced'),
    (2, 6, 'Intermediate'),
    (3, 3, 'Expert'),
    (3, 4, 'Expert'),
    (3, 16, 'Advanced'),
    (9, 21, 'Advanced'),
    (9, 22, 'Intermediate'),
    (9, 23, 'Advanced'),
    (10, 10, 'Advanced'),
    (10, 24, 'Intermediate'),
    (10, 25, 'Advanced'),
    (11, 26, 'Expert'),
    (11, 13, 'Intermediate'),
    (12, 27, 'Expert'),
    (12, 28, 'Advanced'),
    (12, 29, 'Advanced'),
    (13, 11, 'Advanced'),
    (13, 12, 'Intermediate'),
    (14, 13, 'Advanced'),
    (14, 14, 'Intermediate'),
    (15, 30, 'Expert'),
    (15, 31, 'Advanced'),
    (15, 32, 'Intermediate'),
    (16, 33, 'Advanced'),
    (16, 5, 'Advanced'),
    (17, 1, 'Intermediate'),
    (17, 4, 'Intermediate'),
    (17, 34, 'Advanced');
-- =============================================
-- 7. SERVICE PACKAGE 
-- =============================================
SET IDENTITY_INSERT dbo.service_package ON;
INSERT INTO dbo.service_package (
        service_id,
        package_name,
        title,
        price,
        credit,
        max_posts,
        duration_days,
        priority_push,
        has_ai_chatbot,
        description,
        is_active
    )
VALUES (
        1,
        N'Lẻ',
        N'Đăng bài lẻ',
        10000,
        10,
        1,
        NULL,
        0,
        0,
        N'Đăng 1 bài tuyển dụng',
        1
    ),
    (
        2,
        N'Đồng',
        N'Gói Đồng',
        40000,
        40,
        5,
        30,
        1,
        0,
        N'5 bài / 1 tháng',
        1
    ),
    (
        3,
        N'Bạc',
        N'Gói Bạc',
        120000,
        120,
        20,
        90,
        2,
        0,
        N'20 bài / 3 tháng, đẩy bài',
        1
    ),
    (
        4,
        N'Vàng',
        N'Gói Vàng',
        200000,
        200,
        50,
        180,
        3,
        1,
        N'50 bài / 6 tháng + AI Chatbot tư vấn',
        1
    );
SET IDENTITY_INSERT dbo.service_package OFF;
-- =============================================
-- [BỔ SUNG MỚI] 7.1 MUA GÓI (package_transaction)
-- Bắt buộc phải có giao dịch mua gói thành công để tạo lịch sử gói
-- =============================================
SET IDENTITY_INSERT dbo.package_transaction ON;
INSERT INTO dbo.package_transaction (
        transaction_id,
        company_id,
        service_id,
        amount_vnd,
        discount_amount,
        final_amount,
        status,
        transaction_type,
        transaction_date,
        completed_at
    )
VALUES (
        1,
        4,
        3,
        120000,
        0,
        120000,
        'SUCCESS',
        'buy_package',
        GETDATE(),
        GETDATE()
    ),
    -- HR FPT mua gói Bạc
    (
        2,
        5,
        4,
        200000,
        0,
        200000,
        'SUCCESS',
        'buy_package',
        GETDATE(),
        GETDATE()
    );
-- HR Viettel mua gói Vàng
SET IDENTITY_INSERT dbo.package_transaction OFF;
-- =============================================
-- [BỔ SUNG MỚI] 7.2 LỊCH SỬ KÍCH HOẠT GÓI (recruiter_package_history)
-- Điểm trừ bài đăng mẫu sẽ dựa trên ID lịch sử gói này
-- =============================================
SET IDENTITY_INSERT dbo.company_package_history ON;
INSERT INTO dbo.company_package_history (
        id,
        company_id,
        service_id,
        transaction_id,
        posts_granted,
        posts_remaining,
        promotions_remaining,
        is_active,
        start_date,
        end_date,
        price_at_purchase
    )
VALUES (
        1,
        4,
        3,
        1,
        20,
        19,
        3,
        1,
        GETDATE(),
        DATEADD(day, 90, GETDATE()),
        120000
    ),
    -- Kích hoạt gói Bạc cho FPT (id=1)
    (
        2,
        5,
        4,
        2,
        50,
        49,
        3,
        1,
        GETDATE(),
        DATEADD(day, 180, GETDATE()),
        200000
    );
-- Kích hoạt gói Vàng cho Viettel (id=2)
SET IDENTITY_INSERT dbo.company_package_history OFF;
-- =============================================
-- 8. JOB POST (Da dang Working Model + Experience Level)
-- =============================================
SET IDENTITY_INSERT dbo.job_post ON;
INSERT INTO dbo.job_post (
        job_id,
        company_id,
        title,
        position_id,
        company_package_history_id,
        skill,
        working_model,
        salary_min,
        salary_max,
        experience_level,
        description,
        requirement,
        benefit,
        deadline,
        status,
        priority_score,
        moderator_id
    )
VALUES (
        1,
        4,
        N'Senior Java Backend Developer',
        1,
        1,
        N'Microservices, Docker',
        N'Full-time',
        18000000,
        28000000,
        N'Senior',
        N'Tham gia phát triển các dự án lớn cho khách hàng nước ngoài tại FPT Software.',
        N'4+ năm kinh nghiệm Java Spring Boot, hiểu biết về Microservices',
        N'Thưởng dự án, bảo hiểm sức khỏe cao cấp, du lịch hàng năm',
        '2026-07-30',
        'APPROVED',
        85,
        8
    ),
    (
        2,
        5,
        N'ReactJS Frontend Developer',
        3,
        2,
        N'Redux, Tailwind, TypeScript',
        N'Hybrid',
        13000000,
        21000000,
        N'Mid',
        N'Làm việc trên các sản phẩm công nghệ cao của Viettel.',
        N'2+ năm kinh nghiệm ReactJS và TypeScript, hiểu biết về Redux',
        N'Môi trường năng động, phúc lợi tốt, đào tạo định kỳ',
        '2026-06-20',
        'APPROVED',
        70,
        8
    ),
    (
        3,
        18,
        N'Intern .NET Developer',
        2,
        1,
        N'C#, ASP.NET Core',
        N'Onsite',
        4000000,
        6000000,
        N'Intern',
        N'Cơ hội thực tập tại VNG, học hỏi từ đội ngũ kỹ sư giàu kinh nghiệm.',
        N'Sinh viên năm 3-4 CNTT, biết cơ bản C# hoặc .NET, chăm chỉ học hỏi',
        N'Phụ cấp ăn trưa, mentoring từ senior, cơ hội chuyển chính thức',
        '2026-07-15',
        'APPROVED',
        60,
        8
    ),
    (
        4,
        19,
        N'Fresher Frontend Developer',
        3,
        2,
        N'HTML, CSS, JavaScript',
        N'Full-time',
        7000000,
        12000000,
        N'Fresher',
        N'Gia nhập đội ngũ phát triển web tại VinGroup.',
        N'Tốt nghiệp đại học CNTT, biết HTML/CSS/JS cơ bản, ham học hỏi',
        N'Lương cạnh tranh, bảo hiểm đầy đủ, môi trường hiện đại',
        '2026-08-01',
        'APPROVED',
        55,
        8
    ),
    (
        5,
        20,
        N'Junior Python Developer',
        5,
        1,
        N'Python, Django, REST API',
        N'Remote',
        10000000,
        15000000,
        N'Junior',
        N'Phát triển các API backend cho hệ thống thương mại điện tử tại NashTech.',
        N'1+ năm kinh nghiệm Python, biết Django hoặc FastAPI, hiểu REST API',
        N'Làm việc từ xa 100%, thiết bị hỗ trợ, team quốc tế',
        '2026-07-20',
        'APPROVED',
        65,
        8
    ),
    (
        6,
        21,
        N'DevOps Engineer - Docker & K8s',
        8,
        1,
        N'Docker, Kubernetes, AWS',
        N'Hybrid',
        25000000,
        40000000,
        N'Senior',
        N'Xây dựng và vận hành hạ tầng CI/CD cho hệ thống quy mô lớn tại CMC.',
        N'3+ năm DevOps, thành thạo Docker/K8s, kinh nghiệm AWS hoặc Azure',
        N'Phúc lợi cao cấp, cổ phần công ty, đào tạo chứng chỉ cloud',
        '2026-08-10',
        'APPROVED',
        80,
        8
    ),
    (
        7,
        22,
        N'Mobile Developer - Flutter',
        7,
        1,
        N'Flutter, Dart, Firebase',
        N'Remote',
        14000000,
        22000000,
        N'Mid',
        N'Phát triển ứng dụng mobile đa nền tảng cho khách hàng Bosch Vietnam.',
        N'2+ năm Flutter, biết Dart, kinh nghiệm tích hợp Firebase',
        N'Remote toàn thời gian, team quốc tế, thưởng hiệu quả dự án',
        '2026-07-25',
        'APPROVED',
        72,
        8
    ),
    (
        8,
        4,
        N'QA Engineer - Automation Testing',
        13,
        1,
        N'Selenium, Python, TestNG',
        N'Full-time',
        12000000,
        18000000,
        N'Junior',
        N'Kiểm thử tự động cho các hệ thống phần mềm lớn tại FPT Software.',
        N'1+ năm automation testing, biết Selenium, Python hoặc Java, hiểu Agile',
        N'Học 50 chứng chỉ ISTQB, môi trường chuyên nghiệp, thăng tiến nhanh',
        '2026-07-10',
        'APPROVED',
        58,
        8
    ),
    (
        9,
        5,
        N'Project Manager IT',
        16,
        2,
        N'PMP, Agile, Scrum',
        N'Onsite',
        30000000,
        50000000,
        N'Manager',
        N'Quản lý dự án công nghệ quy mô lớn tại Viettel Group.',
        N'5+ năm quản lý dự án IT, chứng chỉ PMP hoặc PMI, tiếng Anh tốt',
        N'Thu nhập cạnh tranh, xe công ty, bảo hiểm cao cấp, cổ phần',
        '2026-08-15',
        'APPROVED',
        90,
        8
    ),
    (
        10,
        18,
        N'Data Engineer - Big Data',
        18,
        1,
        N'Python, Spark, Hadoop',
        N'Hybrid',
        20000000,
        32000000,
        N'Senior',
        N'Xây dựng pipeline dữ liệu lớn phục vụ phân tích kinh doanh tại VNG.',
        N'3+ năm Data Engineering, thành thạo Spark/Hadoop, biết SQL nâng cao',
        N'Môi trường dữ liệu thú vị, lương cao, đãi ngộ top đầu thị trường',
        '2026-07-28',
        'APPROVED',
        78,
        8
    );
SET IDENTITY_INSERT dbo.job_post OFF;
-- =============================================
-- 8.5 JOB POST PROVINCE
-- =============================================
INSERT INTO dbo.job_post_province (job_id, province_id)
VALUES (1, 1),
    (2, 2),
    (3, 2),
    (4, 1),
    (5, 2),
    (6, 1),
    (7, 2),
    (8, 1),
    (9, 1),
    (10, 2);
-- =============================================
-- 9. JOB TECH STACK 
-- =============================================
INSERT INTO dbo.job_tech_stack (job_id, tech_id)
VALUES (1, 1),
    -- job 1: Java
    (1, 2),
    -- job 1: Spring Boot
    (1, 13),
    -- job 1: Docker
    (2, 7),
    -- job 2: ReactJS
    (2, 10),
    -- job 2: TypeScript
    (3, 3),
    -- job 3: .NET
    (3, 4),
    -- job 3: C#
    (4, 7),
    -- job 4: ReactJS (Frontend)
    (5, 5),
    -- job 5: Python
    (6, 13),
    -- job 6: Docker
    (6, 14),
    -- job 6: Kubernetes
    (6, 15),
    -- job 6: AWS
    (7, 11),
    -- job 7: Flutter
    (8, 29),
    -- job 8: Selenium
    (8, 5),
    -- job 8: Python
    (9, 30),
    -- job 9: SQL
    (10, 5),
    -- job 10: Python
    (10, 18);
-- job 10: PostgreSQL
PRINT '=============================================';
PRINT 'SUCCESS: Seed data completed cleanly!';
PRINT 'Default Account Password for testing: Ab@123456';
PRINT '=============================================';
GO

