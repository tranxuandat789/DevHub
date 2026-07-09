-- =============================================
-- SAMPLE DATA FOR OPTIMIZED DATABASE
-- =============================================
USE ITRecruitmentDB;
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


