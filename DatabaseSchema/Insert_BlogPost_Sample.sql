USE ITRecruitmentDB;
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

