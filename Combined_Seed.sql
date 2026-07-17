SET QUOTED_IDENTIFIER ON;
GO
-- =============================================
-- SAMPLE DATA FOR OPTIMIZED DATABASE
-- =============================================
USE DevHub;
GO
SET NOCOUNT ON;
-- =============================================
-- 1. INSERT MASTER DATA 
-- =============================================
-- 1.1 Danh m?c V? tri cong vi?c (common_job_position)
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
-- 1.2 Danh m?c Cong ngh? (common_technology)
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
-- 1.3 Danh m?c T?nh thanh (province)
;
WITH seed(province_name) AS (
    SELECT v
    FROM (
            VALUES (N'Ha N?i'),
                (N'H? Chi Minh'),
                (N'H?i Phong'),
                (N'?a N?ng'),
                (N'C?n Th?'),
                (N'An Giang'),
                (N'Ba R?a - V?ng Tau'),
                (N'B?c Giang'),
                (N'B?c K?n'),
                (N'B?c Lieu'),
                (N'B?c Ninh'),
                (N'B?n Tre'),
                (N'Binh ??nh'),
                (N'Binh D??ng'),
                (N'Binh Ph??c'),
                (N'Binh Thu?n'),
                (N'Ca Mau'),
                (N'Cao B?ng'),
                (N'??k L?k'),
                (N'??k Nong'),
                (N'?i?n Bien'),
                (N'??ng Nai'),
                (N'??ng Thap'),
                (N'Gia Lai'),
                (N'Ha Giang'),
                (N'Ha Nam'),
                (N'Ha T?nh'),
                (N'H?i D??ng'),
                (N'H?u Giang'),
                (N'Hoa Binh'),
                (N'H?ng Yen'),
                (N'Khanh Hoa'),
                (N'Kien Giang'),
                (N'Kon Tum'),
                (N'Lai Chau'),
                (N'Lam ??ng'),
                (N'L?ng S?n'),
                (N'Lao Cai'),
                (N'Long An'),
                (N'Nam ??nh'),
                (N'Ngh? An'),
                (N'Ninh Binh'),
                (N'Ninh Thu?n'),
                (N'Phu Th?'),
                (N'Phu Yen'),
                (N'Qu?ng Binh'),
                (N'Qu?ng Nam'),
                (N'Qu?ng Ngai'),
                (N'Qu?ng Ninh'),
                (N'Qu?ng Tr?'),
                (N'Soc Tr?ng'),
                (N'S?n La'),
                (N'Tay Ninh'),
                (N'Thai Binh'),
                (N'Thai Nguyen'),
                (N'Thanh Hoa'),
                (N'Th?a Thien Hu?'),
                (N'Ti?n Giang'),
                (N'Tra Vinh'),
                (N'Tuyen Quang'),
                (N'V?nh Long'),
                (N'V?nh Phuc'),
                (N'Yen Bai')
        ) AS t(v)
)
INSERT INTO dbo.province (province_name)
SELECT s.province_name
FROM seed s;
-- =============================================
-- 2. USER ACCOUNT (S? d?ng Filtered Index cho Google_id NULL)
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
        N'Phan Tu?n Anh',
        N'Nam',
        '2005-05-12',
        '0987654321',
        N'Ha N?i',
        12000000,
        18000000,
        N'Ha N?i',
        3,
        85,
        1
    ),
    (
        2,
        N'Tr?n Th? Bich',
        N'N?',
        '1999-08-20',
        '0978123456',
        N'H? Chi Minh',
        10000000,
        16000000,
        N'H? Chi Minh',
        2,
        75,
        1
    ),
    (
        3,
        N'Le V?n C??ng',
        N'Nam',
        '1997-03-15',
        '0967123456',
        N'?a N?ng',
        15000000,
        22000000,
        N'?a N?ng',
        5,
        90,
        1
    ),
    (
        9,
        N'Ph?m V?n ??c',
        N'Nam',
        '1998-11-05',
        '0934567890',
        N'Ha N?i',
        15000000,
        23000000,
        N'Ha N?i',
        4,
        80,
        1
    ),
    (
        10,
        N'Hoang Th? Lan',
        N'N?',
        '2000-02-14',
        '0945678901',
        N'H? Chi Minh',
        9000000,
        14000000,
        N'H? Chi Minh',
        1,
        70,
        1
    ),
    (
        11,
        N'Nguy?n Th? Thu',
        N'N?',
        '1998-07-22',
        '0956789012',
        N'?a N?ng',
        11000000,
        17000000,
        N'?a N?ng',
        3,
        78,
        1
    ),
    (
        12,
        N'?? V?n H?i',
        N'Nam',
        '2001-09-10',
        '0967890123',
        N'Ha N?i',
        18000000,
        26000000,
        N'Ha N?i',
        6,
        92,
        1
    ),
    (
        13,
        N'Bui Trung Kien',
        N'Nam',
        '2000-04-18',
        '0978901234',
        N'H? Chi Minh',
        13000000,
        20000000,
        N'H? Chi Minh',
        2,
        65,
        1
    ),
    (
        14,
        N'V? Th? Kim Anh',
        N'N?',
        '1999-12-30',
        '0989012345',
        N'Ha N?i',
        12000000,
        19000000,
        N'Ha N?i',
        3,
        82,
        1
    ),
    (
        15,
        N'Le Minh Hoang',
        N'Nam',
        '1998-06-25',
        '0990123456',
        N'?a N?ng',
        14000000,
        21000000,
        N'?a N?ng',
        4,
        88,
        1
    ),
    (
        16,
        N'Tr?n Kh?c Tung',
        N'Nam',
        '1997-01-08',
        '0911234567',
        N'H? Chi Minh',
        16000000,
        24000000,
        N'H? Chi Minh',
        5,
        85,
        1
    ),
    (
        17,
        N'??ng Qu?c Huy',
        N'Nam',
        '1999-10-15',
        '0922345678',
        N'Ha N?i',
        10000000,
        15500000,
        N'Ha N?i',
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
        N'Ha N?i',
        N'FPT Software la cong ty cong ngh? hang ??u Vi?t Nam',
        'https://fptsoftware.com',
        N'Ph?n m?m',
        '0101234567',
        4.1,
        45000000,
        1,
        75
    ),
    (
        5,
        N'Viettel Group',
        N'Ha N?i',
        N'T?p ?oan Cong nghi?p - Vi?n thong Quan ??i',
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
        N'H? Chi Minh',
        N'Ngan hang TMCP K? Th??ng Vi?t Nam',
        'https://techcombank.com',
        N'Ngan hang',
        '0109876543',
        3.8,
        35000000,
        0,
        75
    ),
    (
        18,
        N'VNG Corporation',
        N'H? Chi Minh',
        N'Cong ty cong ngh? hang ??u v?i Zalo, ZaloPay...',
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
        N'Ha N?i',
        N'T?p ?oan cong ngh? - o to - b?t ??ng s?n',
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
        N'H? Chi Minh',
        N'Cong ty outsourcing ph?n m?m toan c?u',
        'https://nashtechglobal.com',
        N'Ph?n m?m',
        '0103334445',
        3.7,
        95000000,
        1,
        75
    ),
    (
        21,
        N'CMC Corporation',
        N'Ha N?i',
        N'T?p ?oan cong ngh? CMC',
        'https://cmc.com.vn',
        N'Ph?n m?m & D?ch v? IT',
        '0104445556',
        3.7,
        120000000,
        0,
        75
    ),
    (
        22,
        N'Bosch Vietnam',
        N'H? Chi Minh',
        N'Cong ty cong ngh? ??c t?i Vi?t Nam',
        'https://bosch.com.vn',
        N'O to & Cong ngh?',
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
        N'Ph?m Th? H??ng',
        N'HR Manager',
        '0901234567',
        4,
        1
    ),
    (
        5,
        N'Tr?n Minh Quan',
        N'Talent Acquisition Lead',
        '0912345678',
        5,
        1
    ),
    (
        6,
        N'Le Th? Ng?c Anh',
        N'HR Specialist',
        '0923456789',
        6,
        1
    ),
    (
        18,
        N'Nguy?n Th? Mai',
        N'HR Director',
        '0933456789',
        18,
        1
    ),
    (
        19,
        N'Tr?n Hoang Nam',
        N'Talent Manager',
        '0944567890',
        19,
        1
    ),
    (
        20,
        N'Le Th? Thanh',
        N'HR Manager',
        '0955678901',
        20,
        1
    ),
    (
        21,
        N'Ph?m Minh Chau',
        N'Recruitment Lead',
        '0966789012',
        21,
        1
    ),
    (
        22,
        N'?? Th? Huy?n',
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
        N'Nguy?n V?n Tr??ng',
        'ADMIN'
    ),
    (
        8,
        'moderator_01',
        N'Tr?n Trung Th?c',
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
        N'L?',
        N'??ng bai l?',
        10000,
        10,
        1,
        NULL,
        0,
        0,
        N'??ng 1 bai tuy?n d?ng',
        1
    ),
    (
        2,
        N'??ng',
        N'Goi ??ng',
        40000,
        40,
        5,
        30,
        1,
        0,
        N'5 bai / 1 thang',
        1
    ),
    (
        3,
        N'B?c',
        N'Goi B?c',
        120000,
        120,
        20,
        90,
        2,
        0,
        N'20 bai / 3 thang, ??y bai',
        1
    ),
    (
        4,
        N'Vang',
        N'Goi Vang',
        200000,
        200,
        50,
        180,
        3,
        1,
        N'50 bai / 6 thang + AI Chatbot t? v?n',
        1
    );
SET IDENTITY_INSERT dbo.service_package OFF;
-- =============================================
-- [B? SUNG M?I] 7.1 MUA GOI (package_transaction)
-- B?t bu?c ph?i co giao d?ch mua goi thanh cong ?? t?o l?ch s? goi
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
    -- HR FPT mua goi B?c
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
-- HR Viettel mua goi Vang
SET IDENTITY_INSERT dbo.package_transaction OFF;
-- =============================================
-- [B? SUNG M?I] 7.2 L?CH S? KICH HO?T GOI (recruiter_package_history)
-- ?i?m tr? bai ??ng m?u s? d?a tren ID l?ch s? goi nay
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
    -- Kich ho?t goi B?c cho FPT (id=1)
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
-- Kich ho?t goi Vang cho Viettel (id=2)
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
        N'Tham gia phat tri?n cac d? an l?n cho khach hang n??c ngoai t?i FPT Software.',
        N'4+ n?m kinh nghi?m Java Spring Boot, hi?u bi?t v? Microservices',
        N'Th??ng d? an, b?o hi?m s?c kh?e cao c?p, du l?ch hang n?m',
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
        N'Lam vi?c tren cac s?n ph?m cong ngh? cao c?a Viettel.',
        N'2+ n?m kinh nghi?m ReactJS va TypeScript, hi?u bi?t v? Redux',
        N'Moi tr??ng n?ng ??ng, phuc l?i t?t, ?ao t?o ??nh k?',
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
        N'C? h?i th?c t?p t?i VNG, h?c h?i t? ??i ng? k? s? giau kinh nghi?m.',
        N'Sinh vien n?m 3-4 CNTT, bi?t c? b?n C# ho?c .NET, ch?m ch? h?c h?i',
        N'Ph? c?p ?n tr?a, mentoring t? senior, c? h?i chuy?n chinh th?c',
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
        N'Gia nh?p ??i ng? phat tri?n web t?i VinGroup.',
        N'T?t nghi?p ??i h?c CNTT, bi?t HTML/CSS/JS c? b?n, ham h?c h?i',
        N'L??ng c?nh tranh, b?o hi?m ??y ??, moi tr??ng hi?n ??i',
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
        N'Phat tri?n cac API backend cho h? th?ng th??ng m?i ?i?n t? t?i NashTech.',
        N'1+ n?m kinh nghi?m Python, bi?t Django ho?c FastAPI, hi?u REST API',
        N'Lam vi?c t? xa 100%, thi?t b? h? tr?, team qu?c t?',
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
        N'Xay d?ng va v?n hanh h? t?ng CI/CD cho h? th?ng quy mo l?n t?i CMC.',
        N'3+ n?m DevOps, thanh th?o Docker/K8s, kinh nghi?m AWS ho?c Azure',
        N'Phuc l?i cao c?p, c? ph?n cong ty, ?ao t?o ch?ng ch? cloud',
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
        N'Phat tri?n ?ng d?ng mobile ?a n?n t?ng cho khach hang Bosch Vietnam.',
        N'2+ n?m Flutter, bi?t Dart, kinh nghi?m tich h?p Firebase',
        N'Remote toan th?i gian, team qu?c t?, th??ng hi?u qu? d? an',
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
        N'Ki?m th? t? ??ng cho cac h? th?ng ph?n m?m l?n t?i FPT Software.',
        N'1+ n?m automation testing, bi?t Selenium, Python ho?c Java, hi?u Agile',
        N'H?c 50 ch?ng ch? ISTQB, moi tr??ng chuyen nghi?p, th?ng ti?n nhanh',
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
        N'Qu?n ly d? an cong ngh? quy mo l?n t?i Viettel Group.',
        N'5+ n?m qu?n ly d? an IT, ch?ng ch? PMP ho?c PMI, ti?ng Anh t?t',
        N'Thu nh?p c?nh tranh, xe cong ty, b?o hi?m cao c?p, c? ph?n',
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
        N'Xay d?ng pipeline d? li?u l?n ph?c v? phan tich kinh doanh t?i VNG.',
        N'3+ n?m Data Engineering, thanh th?o Spark/Hadoop, bi?t SQL nang cao',
        N'Moi tr??ng d? li?u thu v?, l??ng cao, ?ai ng? top ??u th? tr??ng',
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


 USE DevHub;
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
USE DevHub;
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

 USE DevHub;
GO
SET NOCOUNT ON;
/* ============================================================
 INSERT APPLICATIONS + CV (for testing)
 - Ch? cho job_post status IN ('APPROVED','CLOSED') c?a recruiter 4 & 5.
 - M?i job gan 3 ?ng vien (xoay vong), m?i application co 1 CV rieng
 (?ung candidate) tr? t?i 1 trong 5 file th?t trong wwwroot/uploads/cvs.
 - 5 CV ???c CHIA ??U round-robin theo t?ng application (arn % 5).
 - Idempotent: reset d? li?u test c? tr??c khi chen l?i.
 ============================================================ */
-- ============================================================
-- 0. RESET d? li?u test c? (?? ch?y l?i s?ch, khong trung)
-- ============================================================
DELETE a
FROM dbo.application a
  JOIN dbo.job_post j ON j.job_id = a.job_id
WHERE j.company_id IN (4, 5)
  AND j.status IN ('APPROVED', 'CLOSED');
-- Xoa cac CV test khong con application nao tham chi?u
DELETE FROM dbo.cv
WHERE title LIKE N'[TEST]%'
  AND cv_id NOT IN (
    SELECT cv_id
    FROM dbo.application
  );
-- ============================================================
-- 1. D?ng b?ng map: 1 dong cho m?i application (job ~ candidate)
--    kem arn (s? th? t? toan c?c) ?? chia ??u 5 file CV.
-- ============================================================
IF OBJECT_ID('tempdb..#Map') IS NOT NULL DROP TABLE #Map;
;
WITH Jobs AS (
  SELECT job_id,
    status,
    approved_at,
    created_at,
    ROW_NUMBER() OVER (
      ORDER BY job_id
    ) - 1 AS jrn
  FROM dbo.job_post
  WHERE company_id IN (4, 5)
    AND status IN ('APPROVED', 'CLOSED')
),
Cands AS (
  SELECT candidate_id,
    ROW_NUMBER() OVER (
      ORDER BY candidate_id
    ) - 1 AS crn,
    COUNT(*) OVER () AS cnt
  FROM dbo.candidate
),
Slots AS (
  SELECT 0 AS slot
  UNION ALL
  SELECT 1
  UNION ALL
  SELECT 2
) -- 3 ?ng vien / job
SELECT j.job_id,
  c.candidate_id,
  j.approved_at,
  j.created_at,
  s.slot,
  CASE
    WHEN j.status = 'APPROVED' THEN CASE
      s.slot
      WHEN 0 THEN'PENDING'
      WHEN 1 THEN'APPROVED'
      ELSE 'REJECTED'
    END
    ELSE CASE
      s.slot
      WHEN 0 THEN'HIRED'
      WHEN 1 THEN'FINISHED'
      ELSE 'REJECTED'
    END
  END AS app_status,
  ROW_NUMBER() OVER (
    ORDER BY j.job_id,
      s.slot
  ) - 1 AS arn,
  -- ?? chia ??u CV
  CAST(NULL AS NVARCHAR(255)) AS cv_title,
  CAST(NULL AS NVARCHAR(500)) AS cv_url INTO #Map
FROM Jobs j
  CROSS JOIN Slots s
  JOIN Cands c ON c.crn = (j.jrn + s.slot) % c.cnt;
-- Gan 5 file CV th?t (chia ??u theo arn % 5) + title duy nh?t theo (job, candidate)
UPDATE m
SET cv_title = N'[TEST] CV J' + CAST(m.job_id AS NVARCHAR(10)) + N'-C' + CAST(m.candidate_id AS NVARCHAR(10)),
  cv_url = f.url
FROM #Map m
  JOIN (
    VALUES (
        0,
        N'/uploads/cvs/1_633d6964-5424-4f62-ae1c-f026d9c518ad.docx'
      ),
      (
        1,
        N'/uploads/cvs/23_5486f90b-dcfe-436e-8012-84348cbd697f.docx'
      ),
      (
        2,
        N'/uploads/cvs/23_88027a26-c81a-4c0d-a981-efb1a50d826b.pdf'
      ),
      (
        3,
        N'/uploads/cvs/23_e8626748-26b4-4859-9af0-5c183050a74f.pdf'
      ),
      (
        4,
        N'/uploads/cvs/24_52e163f8-d099-4fc4-81c7-a57f19fe27c9.docx'
      )
  ) AS f(idx, url) ON f.idx = m.arn % 5;
-- ============================================================
-- 2. Insert CV cho t?ng application (g?n ?ung candidate)
-- ============================================================
INSERT INTO dbo.cv (candidate_id, title, cv_url, skills, is_default)
SELECT m.candidate_id,
  m.cv_title,
  m.cv_url,
  N'K? n?ng theo h? s? ?ng vien',
  0
FROM #Map m;
  PRINT N'?a chen CV test (5 file chia ??u).';
-- ============================================================
-- 3. Insert application, tham chi?u ?ung CV v?a t?o
-- ============================================================
INSERT INTO dbo.application (
    job_id,
    candidate_id,
    cv_id,
    cover_letter,
    status,
    applied_at
  )
SELECT m.job_id,
  m.candidate_id,
  cv.cv_id,
  N'Toi r?t quan tam t?i v? tri nay va mong mu?n ???c ?ong gop cho cong ty.',
  m.app_status,
  DATEADD(
    DAY,
    -(m.slot + 1),
    ISNULL(m.approved_at, m.created_at)
  )
FROM #Map m
  JOIN dbo.cv cv ON cv.candidate_id = m.candidate_id
  AND cv.title = m.cv_title;
PRINT N'?a chen application cho job APPROVED/CLOSED c?a recruiter 4 & 5.';
-- ============================================================
-- 4. ??ng b? application_count
-- ============================================================
UPDATE jp
SET jp.application_count = x.cnt
FROM dbo.job_post jp
  JOIN (
    SELECT job_id,
      COUNT(*) AS cnt
    FROM dbo.application
    GROUP BY job_id
  ) x ON x.job_id = jp.job_id
WHERE jp.company_id IN (4, 5)
  AND jp.status IN ('APPROVED', 'CLOSED');
DROP TABLE #Map;
PRINT N'Hoan t?t.';
GO -- ============================================================
  -- 5. Ki?m tra: phan b? 5 file CV tren cac application
  -- ============================================================
SELECT cv.cv_url,
  COUNT(*) AS total_applications
FROM dbo.application a
  JOIN dbo.job_post j ON j.job_id = a.job_id
  JOIN dbo.cv cv ON cv.cv_id = a.cv_id
WHERE j.company_id IN (4, 5)
  AND j.status IN ('APPROVED', 'CLOSED')
GROUP BY cv.cv_url
ORDER BY cv.cv_url;
GO

 USE DevHub;
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

 USE DevHub;
GO
SET NOCOUNT ON;
/* ============================================================
 Chen interview m?u cho cac application APPROVED (ch?a co interview),
 r?i created_at v? nh?ng ngay TR??C (trong 30 ngay qua) ?? dashboard
 co d? li?u ???ng "L??t ph?ng v?n".
 Idempotent: b? qua application ?a co interview.
 ============================================================ */
;
WITH ApprovedApps AS (
    SELECT a.application_id,
        a.candidate_id,
        r.recruiter_id,
        j.title,
        ROW_NUMBER() OVER (
            ORDER BY a.application_id
        ) AS rn
    FROM dbo.application a
        JOIN dbo.job_post j ON j.job_id = a.job_id
        JOIN dbo.recruiter r ON r.company_id = j.company_id
    WHERE UPPER(a.status) = 'APPROVED'
        AND NOT EXISTS (
            SELECT 1
            FROM dbo.interview i
            WHERE i.application_id = a.application_id
        )
)
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
SELECT application_id,
    candidate_id,
    DATEADD(DAY, -(rn * 2) + 2, GETDATE()),
    -- l?ch PV (lui theo t?ng m?c)
    N'https://meet.google.com/abc-defg-hij',
    N'Phong h?p t?ng 5 - V?n phong cong ty',
    CASE
        WHEN rn % 3 = 0 THEN'FINISHED'
        ELSE 'SCHEDULED'
    END,
    N'Ph?ng v?n vong 1: ' + ISNULL(title, N''),
    DATEADD(DAY, -(rn * 2), GETDATE()),
    -- created_at: r?i m?i 2 ngay lui v? qua kh?
    DATEADD(DAY, -(rn * 2), GETDATE())
FROM ApprovedApps
WHERE rn <= 14;
-- t?i ?a 14 m?c (~28 ngay)
PRINT N'?a cheN' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' interview (r?i cac ngay tr??c).';
GO -- Ki?m tra phan b? theo ngay
SELECT CONVERT(varchar, created_at, 23) AS ngay_tao,
    COUNT(*) AS so_interview
FROM dbo.interview
GROUP BY CONVERT(varchar, created_at, 23)
ORDER BY ngay_tao;
GO

 USE DevHub;
GO
SET NOCOUNT ON;
/* ============================================================
 Blog post m?u.
 - publisher_id, approver_id : moderator id (FK -> admin), l?y ??ng.
 - Idempotent: xoa theo slug tr??c khi chen l?i.
 ============================================================ */
DECLARE @modId INT = (
        SELECT TOP 1 admin_id
        FROM dbo.admin
        WHERE role = 'MODERATOR'
        ORDER BY admin_id
    );
IF @modId IS NULL BEGIN RAISERROR(
    N'Khong tim th?y moderator trong b?ng admin.',
    16,
    1
);
RETURN;
END -- Cleanup ?? ch?y l?i khong b? trung slug (UNIQUE)
DELETE FROM dbo.blog_post
WHERE slug IN (
        'tuong-lai-cua-lap-trinh-vien-ai-trong-ky-nguyen-moi',
        'bi-quyet-viet-cv-chuan-nganh-it-ghi-diem-tuyet-doi',
        'lo-trinh-tro-thanh-fullstack-developer-nam-2026',
        'van-hoa-lam-viec-agile-scrum-tai-cac-cong-ty-cong-nghe',
        'microservices-vs-monolithic-khi-nao-nen-chon-kien-truc-nao',
        'muc-luong-nganh-it-tai-viet-nam-hien-nay-ra-sao',
        'huong-dan-vuot-qua-bai-test-thuat-toan-live-coding',
        'tam-quan-trong-cua-tieng-anh-doi-voi-lap-trinh-vien',
        'bao-mat-web-co-ban-phong-chong-tan-cong-xss-va-sql-injection',
        'co-nen-hoc-net-csharp-vao-nam-2026'
    );
INSERT INTO dbo.blog_post (
        [title],
        [slug],
        [tag],
        [content],
        [thumbnail_url],
        [publisher_id],
        [published_at],
        [created_at],
        [is_published],
        [status]
    )
VALUES (
        N'T??ng lai c?a l?p trinh vien AI trong k? nguyen m?i',
        'tuong-lai-cua-lap-trinh-vien-ai-trong-ky-nguyen-moi',
        'AI',
        N'<p>Tri tu? nhan t?o ?ang thay ??i cach chung ta vi?t code. Cac k? n?ng c?n co bao g?m Machine Learning, Deep Learning va kh? n?ng ?ng d?ng Prompt Engineering vao quy trinh lam vi?c...</p>',
        'https://images.unsplash.com/photo-1677442136019-21780ecad995',
        @modId,
        GETDATE(),
        DATEADD(day, -5, GETDATE()),
        1,
        1
    ),
    (
        N'Bi quy?t vi?t CV chu?n nganh IT ghi ?i?m tuy?t ??i',
        'bi-quyet-viet-cv-chuan-nganh-it-ghi-diem-tuyet-doi',
        'Career',
        N'<p>M?t chi?c CV nganh IT ?n t??ng c?n nh?n m?nh vao d? an th?c t?, cac cong ngh? ?a s? d?ng (Tech stack) va co ???ng d?n ??n Github/Portfolio ca nhan...</p>',
        'https://images.unsplash.com/photo-1586281380349-632531db7ed4',
        @modId,
        GETDATE(),
        DATEADD(day, -10, GETDATE()),
        1,
        1
    ),
    (
        N'L? trinh tr? thanh Fullstack Developer n?m 2026',
        'lo-trinh-tro-thanh-fullstack-developer-nam-2026',
        'Fullstack',
        N'<p>Hanh trinh b?t ??u t? Frontend v?i React/Vue/Angular, sau ?o ti?n t?i Backend v?i Node.js/C#/.NET va cu?i cung la lam quen v?i DevOps/Cloud c?n b?n...</p>',
        'https://images.unsplash.com/photo-1498050108023-c5249f4df085',
        @modId,
        GETDATE(),
        DATEADD(day, -15, GETDATE()),
        1,
        1
    ),
    (
        N'V?n hoa lam vi?c Agile/Scrum t?i cac cong ty cong ngh?',
        'van-hoa-lam-viec-agile-scrum-tai-cac-cong-ty-cong-nghe',
        'Agile',
        N'<p>Hi?u v? Sprint, Daily Stand-up, va cach qu?n ly cong vi?c v?i Jira s? giup sinh vien m?i ra tr??ng hoa nh?p c?c nhanh vao moi tr??ng doanh nghi?p...</p>',
        'https://images.unsplash.com/photo-1531403009284-440f080d1e12',
        @modId,
        GETDATE(),
        DATEADD(day, -20, GETDATE()),
        1,
        1
    ),
    (
        N'Microservices vs Monolithic: Khi nao nen ch?n ki?n truc nao?',
        'microservices-vs-monolithic-khi-nao-nen-chon-kien-truc-nao',
        'Architecture',
        N'<p>Khong ph?i luc nao Microservices c?ng la gi?i phap t?t nh?t. V?i cac d? an nh?, Monolithic v?n mang l?i t?c ?? phat tri?n va tri?n khai nhanh chong nh?t...</p>',
        'https://images.unsplash.com/photo-1555949963-ff9fe0c870eb',
        @modId,
        GETDATE(),
        DATEADD(day, -25, GETDATE()),
        1,
        1
    ),
    (
        N'M?c l??ng nganh IT t?i Vi?t Nam hi?n nay ra sao?',
        'muc-luong-nganh-it-tai-viet-nam-hien-nay-ra-sao',
        'Salary',
        N'<p>Theo bao cao m?i nh?t, sinh vien IT m?i ra tr??ng (Fresher) co th? ??t m?c l??ng t? 10-15 tri?u, trong khi cac chuyen gia co kinh nghi?m (Senior) dao ??ng t? 40-70 tri?u...</p>',
        'https://images.unsplash.com/photo-1554224155-8d04cb21cd6c',
        @modId,
        GETDATE(),
        DATEADD(day, -30, GETDATE()),
        1,
        1
    ),
    (
        N'H??ng d?n v??t qua bai test thu?t toan (Live Coding)',
        'huong-dan-vuot-qua-bai-test-thuat-toan-live-coding',
        'Interview',
        N'<p>Kinh nghi?m th?c chi?n khi g?p bai toan kho: Luon giao ti?p v?i ng??i ph?ng v?n, trinh bay thu?t toan tho (brute force) tr??c khi tim cach t?i ?u...</p>',
        'https://images.unsplash.com/photo-1516116216624-53e697fedbea',
        @modId,
        GETDATE(),
        DATEADD(day, -35, GETDATE()),
        1,
        1
    ),
    (
        N'T?m quan tr?ng c?a Ti?ng Anh ??i v?i L?p trinh vien',
        'tam-quan-trong-cua-tieng-anh-doi-voi-lap-trinh-vien',
        'English',
        N'<p>H?u h?t tai li?u, cong ngh? m?i va cac c?ng ??ng ma ngu?n m? ??u s? d?ng ti?ng Anh. Co ngo?i ng? t?t m? ra c? h?i lam vi?c Remote l??ng ngan ?o...</p>',
        'https://images.unsplash.com/photo-1546410531-ef4ce3ef6420',
        @modId,
        GETDATE(),
        DATEADD(day, -40, GETDATE()),
        1,
        1
    ),
    (
        N'B?o m?t Web c? b?n: Phong ch?ng t?n cong XSS va SQL Injection',
        'bao-mat-web-co-ban-phong-chong-tan-cong-xss-va-sql-injection',
        'Security',
        N'<p>T?n cong cheo trang (XSS) va tiem ma SQL (SQL Injection) la hai l? h?ng kinh ?i?n nh?t. Hay cung xem cach s? d?ng Entity Framework va encode HTML ?? phong ng?a...</p>',
        'https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5',
        @modId,
        GETDATE(),
        DATEADD(day, -45, GETDATE()),
        1,
        1
    ),
    (
        N'Co nen h?c .NET/C# vao n?m 2026?',
        'co-nen-hoc-net-csharp-vao-nam-2026',
        '.NET',
        N'<p>.NET Core ngay cang m?nh m? v?i hi?u n?ng c?c ??nh, co th? ch?y tren c? Linux va Mac. ?ay v?n la s? l?a ch?n s? 1 c?a cac t?p ?oan tai chinh, ngan hang va doanh nghi?p l?n...</p>',
        'https://images.unsplash.com/photo-1550439062-609e1531270e',
        @modId,
        GETDATE(),
        DATEADD(day, -50, GETDATE()),
        1,
        1
    );
PRINT N'?a chen ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' blog post.';
GO


