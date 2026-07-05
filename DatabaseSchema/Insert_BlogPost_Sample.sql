USE DevHub;
GO
SET NOCOUNT ON;
/* ============================================================
 Blog post mẫu.
 - publisher_id, approver_id : moderator id (FK -> admin), lấy động.
 - Idempotent: xóa theo slug trước khi chèn lại.
 ============================================================ */
DECLARE @modId INT = (
        SELECT TOP 1 admin_id
        FROM dbo.admin
        WHERE role = 'MODERATOR'
        ORDER BY admin_id
    );
IF @modId IS NULL BEGIN RAISERROR(
    N'Không tìm thấy moderator trong bảng admin.',
    16,
    1
);
RETURN;
END -- Cleanup để chạy lại không bị trùng slug (UNIQUE)
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
        N'Tương lai của lập trình viên AI trong kỷ nguyên mới',
        'tuong-lai-cua-lap-trinh-vien-ai-trong-ky-nguyen-moi',
        'AI',
        N'<p>Trí tuệ nhân tạo đang thay đổi cách chúng ta viết code. Các kỹ năng cần có bao gồm Machine Learning, Deep Learning và khả năng ứng dụng Prompt Engineering vào quy trình làm việc...</p>',
        'https://images.unsplash.com/photo-1677442136019-21780ecad995',
        @modId,
        GETDATE(),
        DATEADD(day, -5, GETDATE()),
        1,
        1
    ),
    (
        N'Bí quyết viết CV chuẩn ngành IT ghi điểm tuyệt đối',
        'bi-quyet-viet-cv-chuan-nganh-it-ghi-diem-tuyet-doi',
        'Career',
        N'<p>Một chiếc CV ngành IT ấn tượng cần nhấn mạnh vào dự án thực tế, các công nghệ đã sử dụng (Tech stack) và có đường dẫn đến Github/Portfolio cá nhân...</p>',
        'https://images.unsplash.com/photo-1586281380349-632531db7ed4',
        @modId,
        GETDATE(),
        DATEADD(day, -10, GETDATE()),
        1,
        1
    ),
    (
        N'Lộ trình trở thành Fullstack Developer năm 2026',
        'lo-trinh-tro-thanh-fullstack-developer-nam-2026',
        'Fullstack',
        N'<p>Hành trình bắt đầu từ Frontend với React/Vue/Angular, sau đó tiến tới Backend với Node.js/C#/.NET và cuối cùng là làm quen với DevOps/Cloud căn bản...</p>',
        'https://images.unsplash.com/photo-1498050108023-c5249f4df085',
        @modId,
        GETDATE(),
        DATEADD(day, -15, GETDATE()),
        1,
        1
    ),
    (
        N'Văn hóa làm việc Agile/Scrum tại các công ty công nghệ',
        'van-hoa-lam-viec-agile-scrum-tai-cac-cong-ty-cong-nghe',
        'Agile',
        N'<p>Hiểu về Sprint, Daily Stand-up, và cách quản lý công việc với Jira sẽ giúp sinh viên mới ra trường hòa nhập cực nhanh vào môi trường doanh nghiệp...</p>',
        'https://images.unsplash.com/photo-1531403009284-440f080d1e12',
        @modId,
        GETDATE(),
        DATEADD(day, -20, GETDATE()),
        1,
        1
    ),
    (
        N'Microservices vs Monolithic: Khi nào nên chọn kiến trúc nào?',
        'microservices-vs-monolithic-khi-nao-nen-chon-kien-truc-nao',
        'Architecture',
        N'<p>Không phải lúc nào Microservices cũng là giải pháp tốt nhất. Với các dự án nhỏ, Monolithic vẫn mang lại tốc độ phát triển và triển khai nhanh chóng nhất...</p>',
        'https://images.unsplash.com/photo-1555949963-ff9fe0c870eb',
        @modId,
        GETDATE(),
        DATEADD(day, -25, GETDATE()),
        1,
        1
    ),
    (
        N'Mức lương ngành IT tại Việt Nam hiện nay ra sao?',
        'muc-luong-nganh-it-tai-viet-nam-hien-nay-ra-sao',
        'Salary',
        N'<p>Theo báo cáo mới nhất, sinh viên IT mới ra trường (Fresher) có thể đạt mức lương từ 10-15 triệu, trong khi các chuyên gia có kinh nghiệm (Senior) dao động từ 40-70 triệu...</p>',
        'https://images.unsplash.com/photo-1554224155-8d04cb21cd6c',
        @modId,
        GETDATE(),
        DATEADD(day, -30, GETDATE()),
        1,
        1
    ),
    (
        N'Hướng dẫn vượt qua bài test thuật toán (Live Coding)',
        'huong-dan-vuot-qua-bai-test-thuat-toan-live-coding',
        'Interview',
        N'<p>Kinh nghiệm thực chiến khi gặp bài toán khó: Luôn giao tiếp với người phỏng vấn, trình bày thuật toán thô (brute force) trước khi tìm cách tối ưu...</p>',
        'https://images.unsplash.com/photo-1516116216624-53e697fedbea',
        @modId,
        GETDATE(),
        DATEADD(day, -35, GETDATE()),
        1,
        1
    ),
    (
        N'Tầm quan trọng của Tiếng Anh đối với Lập trình viên',
        'tam-quan-trong-cua-tieng-anh-doi-voi-lap-trinh-vien',
        'English',
        N'<p>Hầu hết tài liệu, công nghệ mới và các cộng đồng mã nguồn mở đều sử dụng tiếng Anh. Có ngoại ngữ tốt mở ra cơ hội làm việc Remote lương ngàn đô...</p>',
        'https://images.unsplash.com/photo-1546410531-ef4ce3ef6420',
        @modId,
        GETDATE(),
        DATEADD(day, -40, GETDATE()),
        1,
        1
    ),
    (
        N'Bảo mật Web cơ bản: Phòng chống tấn công XSS và SQL Injection',
        'bao-mat-web-co-ban-phong-chong-tan-cong-xss-va-sql-injection',
        'Security',
        N'<p>Tấn công chéo trang (XSS) và tiêm mã SQL (SQL Injection) là hai lỗ hổng kinh điển nhất. Hãy cùng xem cách sử dụng Entity Framework và encode HTML để phòng ngừa...</p>',
        'https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5',
        @modId,
        GETDATE(),
        DATEADD(day, -45, GETDATE()),
        1,
        1
    ),
    (
        N'Có nên học .NET/C# vào năm 2026?',
        'co-nen-hoc-net-csharp-vao-nam-2026',
        '.NET',
        N'<p>.NET Core ngày càng mạnh mẽ với hiệu năng cực đỉnh, có thể chạy trên cả Linux và Mac. Đây vẫn là sự lựa chọn số 1 của các tập đoàn tài chính, ngân hàng và doanh nghiệp lớn...</p>',
        'https://images.unsplash.com/photo-1550439062-609e1531270e',
        @modId,
        GETDATE(),
        DATEADD(day, -50, GETDATE()),
        1,
        1
    );
PRINT N'Đã chèn ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' blog post.';
GO
