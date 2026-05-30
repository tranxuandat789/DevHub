# Ghi chú Schema Database — DevHub

> Tệp này mô tả các bảng, ràng buộc (constraints), quan hệ giữa các bảng (1-n, 1-1), và chú thích cho một số trường khó hiểu hoặc quan trọng.

## Tổng quan
- CSDL: `ITRecruitmentDB`.
- Bảng chính: `user_account` làm bảng người dùng cơ sở; các bảng `admin`, `candidate`, `recruiter` mở rộng thông tin người dùng bằng khóa chính là `user_id`.

---

## Danh sách bảng & mô tả

### 1) `user_account`
- Mục đích: Bảng người dùng chung (tài khoản).
- Khóa chính: `user_id` (IDENTITY).
- Các cột chính: `email` (UNIQUE, NOT NULL), `password_hash`, `google_id` (UNIQUE WHERE NOT NULL), `user_type` (CHECK IN: 'ADMIN','RECRUITER','MODERATOR','CANDIDATE'), `is_active`, `created_at`, `last_login`, `last_updated`.
- Ràng buộc quan trọng:
  - Unique: `email` (toàn bộ), `google_id` (phiên bản NONCLUSTERED index với WHERE google_id IS NOT NULL).
  - Check: `user_type` phải nằm trong danh sách enum đã cho.
- Quan hệ:
  - 1-1 tới `admin` (`admin.admin_id` PK = FK -> `user_account.user_id`).
  - 1-1 tới `candidate` và `recruiter` theo cùng mô hình (PK là FK đến `user_account`).

Chú ý: `google_id` optional nhưng unique nếu tồn tại (hỗ trợ đăng nhập Google).

---

### 2) `admin`
- Mục đích: Thông tin admin/moderator.
- PK: `admin_id` (cũng là FK -> `user_account.user_id`).
- Cột: `username` (UNIQUE), `full_name`, `role` (CHECK IN ('ADMIN','MODERATOR')).
- Quan hệ: 1-1 với `user_account` (một bản ghi admin ứng với một tài khoản user).

---

### 3) `candidate`
- Mục đích: Hồ sơ ứng viên mở rộng.
- PK: `candidate_id` (cũng là FK -> `user_account.user_id`).
- Các cột quan trọng: `full_name`, `birthdate`, `phone`, `expected_salary_min`, `expected_salary_max`, `experience_years`, `cv_searchable` (flag), `profile_completion`, `image_url`.
- Quan hệ: 1-1 với `user_account`.
- ON DELETE: FK thiết lập `ON DELETE NO ACTION` (tức xóa user không tự động xóa candidate)

Ghi chú: `cv_searchable` có thể định nghĩa là cho phép CV xuất hiện trong kết quả tìm kiếm (boolean).

---

### 4) `recruiter`
- Mục đích: Thông tin nhà tuyển dụng (mở rộng user).
- PK: `recruiter_id` (FK -> `user_account.user_id`).
- Các cột quan trọng: `company_name`, `website`, `industry`, `tax_code`, `total_spent`, `average_rating`, `is_verified`, `profile_completion`.
- Quan hệ: 1-1 với `user_account`.

---

### 5) `common_job_position`
- Mục đích: Danh mục vị trí chung (reference cho job_post).
- PK: `position_id` (IDENTITY), `position_name` UNIQUE.
- Quan hệ: 1-n — một `position` có thể được nhiều `job_post` tham chiếu.

---

### 6) `common_technology`
- Mục đích: Danh mục công nghệ/skill.
- PK: `tech_id` (IDENTITY), `tech_name` UNIQUE.
- Quan hệ: 1-n tới `candidate_skill`, 1-n tới `job_tech_stack`.

---

### 7) `cv`
- Mục đích: Lưu LINK CV của ứng viên.
- PK: `cv_id` (IDENTITY).
- FK: `candidate_id` -> `candidate(candidate_id)` ON DELETE CASCADE (xóa candidate sẽ xóa CV).
- Cột quan trọng: `cv_url`, `is_default` (flag), `skills`, `experience`, `education`, `languages`, `created_at`, `updated_at`.
- Quan hệ: candidate 1-n cv (một ứng viên có nhiều CV).

Ghi chú: Không có ràng buộc thêm để đảm bảo chỉ một `is_default = 1` cho mỗi ứng viên — đây là điểm cần lưu ý/khắc phục nếu muốn nhất quán.

---

### 8) `candidate_skill`
- Mục đích: Bảng quan hệ giữa `candidate` và `common_technology` (join table).
- PK: Composite (`candidate_id`, `tech_id`).
- FK: `candidate_id` -> `candidate` ON DELETE CASCADE; `tech_id` -> `common_technology` ON DELETE CASCADE.
- Quan hệ: candidate 1-n candidate_skill; technology 1-n candidate_skill. Về khái niệm: candidate ↔ technology là Many-to-Many.

---

### 9) `service_package`
- Mục đích: Các gói dịch vụ mà recruiter có thể mua.
- PK: `service_id`.
- Cột: `package_name`, `price`, `credit`, `max_posts`, `duration_days`, `priority_push`, `has_ai_chatbot`, `is_active`.

---

### 10) `promotion`
- Mục đích: Mã khuyến mãi.
- PK: `promotion_id`, `promo_code` UNIQUE, `discount_percent`, `max_discount`, `start_date`, `end_date`.

---

### 11) `package_transaction`
- Mục đích: Giao dịch thanh toán (VNPay).
- PK: `transaction_id`.
- FK: `recruiter_id` -> `recruiter` ON DELETE CASCADE; `service_id` -> `service_package`; `promotion_id` -> `promotion`.
- Các cột quan trọng: `amount_vnd`, `discount_amount`, `final_amount`, `payment_method` (default 'vnpay'), `vnpay_txn_ref`, `vnpay_transaction_no`, `status` (CHECK IN ('PENDING','SUCCESS','FAILED','CANCELLED','REFUNDED')), `transaction_type`, `transaction_date`, `completed_at`.
- Indexes: `idx_payment_recruiter`, `idx_payment_status`, `idx_payment_vnpay_ref`.

Quan hệ: recruiter 1-n package_transaction.

---

### 12) `recruiter_package_history`
- Mục đích: Lịch sử gói dịch vụ đã kích hoạt cho recruiter.
- PK: `id`.
- FK: `recruiter_id` -> `recruiter` ON DELETE CASCADE; `service_id` -> `service_package` ON DELETE CASCADE; `transaction_id` -> `package_transaction` (ON DELETE NO ACTION).
- Cột quan trọng: `posts_granted`, `posts_remaining`, `promotions_remaining`, `start_date`, `end_date`, `price_at_purchase`.

Quan hệ: recruiter 1-n recruiter_package_history; service_package 1-n recruiter_package_history; package_transaction 1-n recruiter_package_history (schema cho phép nhiều history tham chiếu cùng 1 transaction).

---

### 13) `job_post`
- Mục đích: Tin tuyển dụng.
- PK: `job_id`.
- FK: `recruiter_id` -> `recruiter`; `position_id` -> `common_job_position` ON DELETE CASCADE; `moderator_id` -> `admin`; `recruiter_package_history_id` -> `recruiter_package_history`.
- Các cột quan trọng: `title`, `description`, `skill`, `experience_level`, `location`, `working_model`, `salary_min`, `salary_max`, `hiring_quota`, `deadline`, `status` (CHECK IN ...), `priority_score`, `application_count`, `approved_at`, `rejected_reason`, `created_at`.
- Indexes: `idx_job_post_recruiter`, `idx_job_post_status`.

Quan hệ: recruiter 1-n job_post; position 1-n job_post; moderator (admin) 1-n job_post.

---

### 14) `job_tech_stack`
- Mục đích: Bảng many-to-many giữa `job_post` và `common_technology`.
- PK: composite (`job_id`, `tech_id`).
- FK: `job_id` -> `job_post` ON DELETE CASCADE; `tech_id` -> `common_technology` ON DELETE CASCADE.
- Quan hệ: job_post ↔ common_technology là Many-to-Many.

---

### 15) `application`
- Mục đích: Ứng tuyển ứng viên cho job.
- PK: `application_id`.
- FK: `job_id` -> `job_post` ON DELETE CASCADE; `candidate_id` -> `candidate` ON DELETE NO ACTION; `cv_id` -> `cv` ON DELETE NO ACTION.
- Cột: `cover_letter`, `notes`, `status` (CHECK IN ...), `applied_at`.
- Indexes: `idx_application_job`, `idx_application_candidate`.

Quan hệ: job_post 1-n application; candidate 1-n application; cv 1-n application.

---

### 16) `interview`
- Mục đích: Lịch phỏng vấn cho ứng dụng.
- PK: `interview_id`.
- FK: `application_id` -> `application` ON DELETE CASCADE; `candidate_id` -> `candidate`; `recruiter_id` -> `recruiter`.
- Cột: `location`, `meeting_link`, `scheduled_time`, `notes`, `status` (CHECK IN ...), `created_at`, `updated_at`.

Quan hệ: application 1-n interview; candidate 1-n interview; recruiter 1-n interview.

---

### 17) `bookmark`
- Mục đích: Ứng viên đánh dấu tin tuyển dụng.
- PK: `bookmark_id`.
- FK: `candidate_id` -> `candidate` ON DELETE CASCADE; `job_id` -> `job_post` ON DELETE CASCADE.
- Quan hệ: candidate 1-n bookmark; job_post 1-n bookmark.

---

### 18) `review_recruiter`
- Mục đích: Đánh giá nhà tuyển dụng bởi ứng viên.
- PK: `review_id`.
- Unique: `UQ_candidate_recruiter_review` trên (`candidate_id`, `recruiter_id`) — tức mỗi cặp candidate/recruiter chỉ có 1 đánh giá.
- FK: `candidate_id` -> `candidate` ON DELETE CASCADE; `recruiter_id` -> `recruiter` ON DELETE CASCADE; `moderator_id` -> `admin` ON DELETE NO ACTION.
- Cột quan trọng: `rating`, `pros`, `cons`, `description`, `is_anonymous`, `status` (CHECK IN ('PENDING','APPROVED','REJECTED')).

Quan hệ: recruiter 1-n review_recruiter; candidate 1-n review_recruiter, nhưng enforced uniqueness khiến mỗi candidate chỉ đánh giá 1 lần cho cùng 1 recruiter (1-1 per pair).

---

### 19) `blog_post`
- Mục đích: Bài viết blog.
- PK: `blog_id`.
- FK: `publisher_id` -> `admin` ON DELETE CASCADE; `author_id` -> `recruiter` ON DELETE SET NULL.
- Unique: `slug`.
- Cột: `title`, `author`, `content`, `thumbnail_url`, `is_published`, `created_at`, `published_at`.

Quan hệ: admin (publisher) 1-n blog_post; recruiter (author) 1-n blog_post (nullable).

---

### 20) `notification`
- Mục đích: Thông báo hệ thống (có thể polymorphic với `reference_type`/`reference_id`).
- PK: `notification_id`.
- Cột: `user_id`, `user_type`, `type`, `title`, `message`, `reference_type`, `reference_id`, `severity_level`, `is_read`, `created_at`.
- Index: `idx_notification_user` trên (`user_id`, `user_type`).

Ghi chú: `reference_type`/`reference_id` là pattern tham chiếu đa hình (ví dụ: job_id, application_id, ...).

---

### 21) `audit_log`
- Mục đích: Lưu lịch sử thao tác/đổi trạng thái.
- PK: `log_id`.
- Cột: `user_id`, `user_type`, `entity_type`, `entity_id`, `action`, `old_value`, `new_value`, `ip_address`, `created_at`.

---

## Tổng hợp quan hệ chính (tóm tắt)
- `user_account` 1-1 `admin` (một user có thể là admin; admin PK là user_id).
- `user_account` 1-1 `candidate`.
- `user_account` 1-1 `recruiter`.
- `candidate` 1-n `cv`.
- `candidate` 1-n `application`.
- `job_post` 1-n `application`.
- `job_post` many-to-many `common_technology` thông qua `job_tech_stack`.
- `candidate` many-to-many `common_technology` thông qua `candidate_skill`.
- `recruiter` 1-n `job_post`, `package_transaction`, `recruiter_package_history`, `review_recruiter`, `interview`.
- `promotion` 1-n `package_transaction`.
- `service_package` 1-n `package_transaction` và 1-n `recruiter_package_history`.

---

## Các ràng buộc CHECK & ENUM-like fields
- `user_account.user_type` ∈ ('ADMIN','RECRUITER','MODERATOR','CANDIDATE')
- `admin.role` ∈ ('ADMIN','MODERATOR')
- `package_transaction.status` ∈ ('PENDING','SUCCESS','FAILED','CANCELLED','REFUNDED')
- `job_post.status` ∈ ('PENDING','APPROVED','REJECTED','FINISHED','CLOSED','EXPIRED')
- `application.status` ∈ ('PENDING','APPROVED','REJECTED','FINISHED','HIRED','CANCELLED','FAILED')
- `interview.status` ∈ ('SCHEDULED','PENDING','FINISHED','EXPIRED','CANCELLED')
- `review_recruiter.status` ∈ ('PENDING','APPROVED','REJECTED')

---

## Chú thích cho một số trường có thể khó hiểu
- `google_id` (user_account): ID đăng nhập của Google — optional, unique khi có.
- `cv_searchable` (candidate): boolean, khả năng CV được hiển thị trong tìm kiếm hồ sơ (0/1).
- `is_default` (cv): flag xác định CV mặc định; hiện không có ràng buộc đảm bảo chỉ một CV mặc định trên một ứng viên.
- `vnpay_txn_ref` vs `vnpay_transaction_no` (package_transaction): khả năng `vnpay_txn_ref` là tham chiếu giao dịch do hệ thống VNPay trả về, còn `vnpay_transaction_no` có thể là mã giao dịch bên bên ngân hàng/VNPay; cần kiểm tra thêm bên tích hợp VNPay.
- `transaction_type` (package_transaction): không có ENUM CHECK — có thể là 'TOPUP','PURCHASE',... (kiểm tra code ứng dụng để biết giá trị thực tế).
- `recruiter_package_history.transaction_id`: không unique — schema cho phép nhiều bản ghi lịch sử tham chiếu cùng một transaction; nếu mong muốn là 1-1, cần thêm UNIQUE constraint.
- `reference_type` / `reference_id` (notification): pattern tham chiếu đa hình; cần định nghĩa cách dùng và danh sách `reference_type` hợp lệ trong ứng dụng.
- `priority_score` (job_post): điểm uu tien hiển thị (phu thuoc vao service package).

---

## Các điểm cần lưu ý / đề xuất cải thiện
- Thêm constraint/logic để đảm bảo chỉ một `cv.is_default = 1` cho mỗi `candidate` (ví dụ: unique filtered index ON (candidate_id) WHERE is_default = 1).
- Quy định rõ `vnpay_txn_ref` vs `vnpay_transaction_no` trong tài liệu tích hợp VNPay.
- Nếu muốn chắc chắn rằng `recruiter_package_history.transaction_id` khớp 1-1 với `package_transaction`, thêm UNIQUE constraint trên `transaction_id` trong `recruiter_package_history`.
- Cân nhắc ON DELETE behavior cho FK `candidate` và `recruiter` từ `user_account` — hiện `NO ACTION` nghĩa là xóa user không tự động dọn các bản ghi mở rộng; có thể muốn `ON DELETE CASCADE` hoặc xử lý ở ứng dụng.
- Xem xét thêm CHECK/ENUM cho `transaction_type` và `payment_method` nếu giá trị cố định.

---

## Các trường quan trọng (tóm tắt để dev/QA chú ý)
- `user_account.email`, `user_account.password_hash`, `user_account.user_type`, `user_account.google_id`.
- `candidate.expected_salary_min` / `expected_salary_max`, `candidate.experience_years`, `candidate.profile_completion`.
- `recruiter.company_name`, `recruiter.is_verified`, `recruiter.total_spent`, `recruiter.average_rating`.
- `job_post.position_id`, `job_post.recruiter_id`, `job_post.status`, `job_post.salary_min`/`salary_max`, `job_post.application_count`.
- `application.status`, `application.applied_at`, `application.cv_id`.
- `package_transaction.final_amount`, `package_transaction.status`, `package_transaction.recruiter_id`.
- `recruiter_package_history.posts_remaining`, `recruiter_package_history.price_at_purchase`.
- `review_recruiter.rating` và `UQ_candidate_recruiter_review` (đảm bảo 1 đánh giá/cặp).

---

## Kết luận
Tài liệu này tóm tắt cấu trúc hiện tại của schema và chỉ ra vài chỗ cần xem xét (ràng buộc uniqueness cho `cv.is_default`, tính nhất quán `transaction_id` ↔ `recruiter_package_history`, enum/check bổ sung cho `transaction_type`).

Nếu bạn muốn, tôi có thể:
- Thêm các diagram ERD (Mermaid) mô tả quan hệ chính.
- Tạo các ALTER TABLE đề xuất để thêm unique/check indexes (ví dụ: unique filtered index cho `is_default`).

--
Nhỏ: file sinh tự động từ `Database_Schema.sql` — kiểm tra lại nếu schema thay đổi sau này.
