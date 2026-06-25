using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredWorkingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    user_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    entity_id = table.Column<int>(type: "int", nullable: true),
                    old_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__audit_lo__9E2397E0562E6B47", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "common_job_position",
                columns: table => new
                {
                    position_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    position_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__common_j__99A0E7A487B920C3", x => x.position_id);
                });

            migrationBuilder.CreateTable(
                name: "common_technology",
                columns: table => new
                {
                    tech_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tech_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__common_t__E0D7817A3C28CA0C", x => x.tech_id);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    user_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    severity_level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Low"),
                    reference_id = table.Column<int>(type: "int", nullable: true),
                    reference_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F06EAA605", x => x.notification_id);
                });

            migrationBuilder.CreateTable(
                name: "promotion",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    promo_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    discount_percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    max_discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__promotio__2CB9556B27C78245", x => x.promotion_id);
                });

            migrationBuilder.CreateTable(
                name: "service_package",
                columns: table => new
                {
                    service_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    package_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    credit = table.Column<int>(type: "int", nullable: false),
                    max_posts = table.Column<int>(type: "int", nullable: false),
                    duration_days = table.Column<int>(type: "int", nullable: true),
                    priority_push = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    has_ai_chatbot = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__service___3E0DB8AF78E35848", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "user_account",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    google_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    otp_verification = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    otp_expires_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    reset_password_token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    reset_password_expires_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    user_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_login = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_acc__B9BE370FB27D0E4F", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    admin_id = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__admin__43AA41418C23432B", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK__admin__admin_id__571DF1D5",
                        column: x => x.admin_id,
                        principalTable: "user_account",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "candidate",
                columns: table => new
                {
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    expected_salary_min = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    expected_salary_max = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    preferred_location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    preferred_working_model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    experience_years = table.Column<int>(type: "int", nullable: true),
                    cv_searchable = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    profile_completion = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    social_media_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__candidat__39BD4C187D159B41", x => x.candidate_id);
                    table.ForeignKey(
                        name: "FK__candidate__candi__49C3F6B7",
                        column: x => x.candidate_id,
                        principalTable: "user_account",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "recruiter",
                columns: table => new
                {
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    company_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    company_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    company_logo_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    company_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tax_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    business_license_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    additional_documents_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    total_spent = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m),
                    average_rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true, defaultValue: 0.00m),
                    total_reviews = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    is_verified = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    profile_completion = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__recruite__42ABA2576945C9D7", x => x.recruiter_id);
                    table.ForeignKey(
                        name: "FK__recruiter__recru__534D60F1",
                        column: x => x.recruiter_id,
                        principalTable: "user_account",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "candidate_skill",
                columns: table => new
                {
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    tech_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__candidat__87B0340FA344E306", x => new { x.candidate_id, x.tech_id });
                    table.ForeignKey(
                        name: "FK__candidate__candi__68487DD7",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__candidate__tech___693CA210",
                        column: x => x.tech_id,
                        principalTable: "common_technology",
                        principalColumn: "tech_id");
                });

            migrationBuilder.CreateTable(
                name: "cv",
                columns: table => new
                {
                    cv_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    experience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    skills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cv_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_default = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__cv__C36883E6D4E21DAA", x => x.cv_id);
                    table.ForeignKey(
                        name: "FK__cv__candidate_id__6EF57B66",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                });

            migrationBuilder.CreateTable(
                name: "blog_post",
                columns: table => new
                {
                    blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thumbnail_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    author_id = table.Column<int>(type: "int", nullable: true),
                    publisher_id = table.Column<int>(type: "int", nullable: false),
                    is_published = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    published_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__blog_pos__2975AA28BA813DA3", x => x.blog_id);
                    table.ForeignKey(
                        name: "FK__blog_post__publi__395884C4",
                        column: x => x.publisher_id,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                    table.ForeignKey(
                        name: "FK_blog_post_recruiter_author_id",
                        column: x => x.author_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "package_transaction",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: true),
                    amount_vnd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    final_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "vnpay"),
                    vnpay_txn_ref = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vnpay_transaction_no = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vnpay_bank_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "pending"),
                    transaction_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    transaction_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    completed_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__package___85C600AFBBD9338E", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK__package_transaction__promotion",
                        column: x => x.promotion_id,
                        principalTable: "promotion",
                        principalColumn: "promotion_id");
                    table.ForeignKey(
                        name: "FK__package_transaction__recruiter",
                        column: x => x.recruiter_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__package_transaction__service",
                        column: x => x.service_id,
                        principalTable: "service_package",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateTable(
                name: "review_recruiter",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    pros = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_anonymous = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    rejection_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    moderator_id = table.Column<int>(type: "int", nullable: true),
                    moderated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__review_r__60883D90CFCBFACD", x => x.review_id);
                    table.ForeignKey(
                        name: "FK__review_re__candi__2DE6D218",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__review_re__recru__2EDAF651",
                        column: x => x.recruiter_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id");
                    table.ForeignKey(
                        name: "FK_review_recruiter_admin_moderator_id",
                        column: x => x.moderator_id,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                });

            migrationBuilder.CreateTable(
                name: "recruiter_package_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false),
                    transaction_id = table.Column<int>(type: "int", nullable: false),
                    posts_granted = table.Column<int>(type: "int", nullable: false),
                    posts_remaining = table.Column<int>(type: "int", nullable: false),
                    promotions_remaining = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    price_at_purchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__recruite__3213E83FA34D72CF", x => x.id);
                    table.ForeignKey(
                        name: "FK__recruiter_history__recruiter",
                        column: x => x.recruiter_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__recruiter_history__service",
                        column: x => x.service_id,
                        principalTable: "service_package",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__recruiter_history__transaction",
                        column: x => x.transaction_id,
                        principalTable: "package_transaction",
                        principalColumn: "transaction_id");
                });

            migrationBuilder.CreateTable(
                name: "job_post",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    position_id = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    skill = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    working_model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    salary_min = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    salary_max = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    experience_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requirement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    benefit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    deadline = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "PENDING"),
                    priority_score = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    hiring_quota = table.Column<int>(type: "int", nullable: true),
                    application_count = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    approved_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    moderator_id = table.Column<int>(type: "int", nullable: true),
                    recruiter_package_history_id = table.Column<int>(type: "int", nullable: false),
                    rejected_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__job_post__6E32B6A56D928E5F", x => x.job_id);
                    table.ForeignKey(
                        name: "FK__job_post__modera__619B8048",
                        column: x => x.moderator_id,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                    table.ForeignKey(
                        name: "FK__job_post__package_history",
                        column: x => x.recruiter_package_history_id,
                        principalTable: "recruiter_package_history",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__job_post__positi__60A75C0F",
                        column: x => x.position_id,
                        principalTable: "common_job_position",
                        principalColumn: "position_id");
                    table.ForeignKey(
                        name: "FK__job_post__recruiter",
                        column: x => x.recruiter_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id");
                });

            migrationBuilder.CreateTable(
                name: "application",
                columns: table => new
                {
                    application_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    cv_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    cover_letter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    applied_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, defaultValue: "pending"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__applicat__3BCBDCF26F68278F", x => x.application_id);
                    table.ForeignKey(
                        name: "FK__applicati__candi__73BA3083",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__applicati__cv_id__74AE54BC",
                        column: x => x.cv_id,
                        principalTable: "cv",
                        principalColumn: "cv_id");
                    table.ForeignKey(
                        name: "FK__applicati__job_i__75A278F5",
                        column: x => x.job_id,
                        principalTable: "job_post",
                        principalColumn: "job_id");
                });

            migrationBuilder.CreateTable(
                name: "bookmark",
                columns: table => new
                {
                    bookmark_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    job_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bookmark__D9C65802FC133ABD", x => x.bookmark_id);
                    table.ForeignKey(
                        name: "FK__bookmark__candid__32AB8735",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__bookmark__job_id__339FAB6E",
                        column: x => x.job_id,
                        principalTable: "job_post",
                        principalColumn: "job_id");
                });

            migrationBuilder.CreateTable(
                name: "job_tech_stack",
                columns: table => new
                {
                    job_id = table.Column<int>(type: "int", nullable: false),
                    tech_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__job_tech__D03FCEB2D9718A37", x => new { x.job_id, x.tech_id });
                    table.ForeignKey(
                        name: "FK__job_tech___job_i__6477ECF3",
                        column: x => x.job_id,
                        principalTable: "job_post",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__job_tech___tech___656C112C",
                        column: x => x.tech_id,
                        principalTable: "common_technology",
                        principalColumn: "tech_id");
                });

            migrationBuilder.CreateTable(
                name: "interview",
                columns: table => new
                {
                    interview_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    application_id = table.Column<int>(type: "int", nullable: false),
                    recruiter_id = table.Column<int>(type: "int", nullable: false),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    scheduled_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    meeting_link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "scheduled"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__intervie__141E55525829E99E", x => x.interview_id);
                    table.ForeignKey(
                        name: "FK__interview__appli__7C4F7684",
                        column: x => x.application_id,
                        principalTable: "application",
                        principalColumn: "application_id");
                    table.ForeignKey(
                        name: "FK__interview__candi__7E37BEF6",
                        column: x => x.candidate_id,
                        principalTable: "candidate",
                        principalColumn: "candidate_id");
                    table.ForeignKey(
                        name: "FK__interview__recru__7D439ABD",
                        column: x => x.recruiter_id,
                        principalTable: "recruiter",
                        principalColumn: "recruiter_id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__admin__F3DBC5727CFAE85F",
                table: "admin",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_application_candidate",
                table: "application",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "idx_application_job",
                table: "application",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_cv_id",
                table: "application",
                column: "cv_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_author_id",
                table: "blog_post",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_publisher_id",
                table: "blog_post",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "UQ__blog_pos__32DD1E4C47481A2E",
                table: "blog_post",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookmark_candidate_id",
                table: "bookmark",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookmark_job_id",
                table: "bookmark",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "IX_candidate_skill_tech_id",
                table: "candidate_skill",
                column: "tech_id");

            migrationBuilder.CreateIndex(
                name: "UQ__common_j__0030EAD7A7C738AF",
                table: "common_job_position",
                column: "position_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__common_t__AC1EA1F13AC03AE9",
                table: "common_technology",
                column: "tech_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cv_candidate_id",
                table: "cv",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_interview_application_id",
                table: "interview",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_interview_candidate_id",
                table: "interview",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_interview_recruiter_id",
                table: "interview",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "idx_job_post_recruiter",
                table: "job_post",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "idx_job_post_status",
                table: "job_post",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_job_post_moderator_id",
                table: "job_post",
                column: "moderator_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_post_position_id",
                table: "job_post",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_job_post_recruiter_package_history_id",
                table: "job_post",
                column: "recruiter_package_history_id");

            migrationBuilder.CreateIndex(
                name: "idx_job_tech_stack_lookup",
                table: "job_tech_stack",
                columns: new[] { "tech_id", "job_id" });

            migrationBuilder.CreateIndex(
                name: "idx_notification_user",
                table: "notification",
                columns: new[] { "user_id", "user_type" });

            migrationBuilder.CreateIndex(
                name: "IX_package_transaction_promotion_id",
                table: "package_transaction",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_package_transaction_recruiter_id",
                table: "package_transaction",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "IX_package_transaction_service_id",
                table: "package_transaction",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "UQ__promotio__C07E23151D5A3120",
                table: "promotion",
                column: "promo_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recruiter_package_history_recruiter_id",
                table: "recruiter_package_history",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "IX_recruiter_package_history_service_id",
                table: "recruiter_package_history",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_recruiter_package_history_transaction_id",
                table: "recruiter_package_history",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_recruiter_moderator_id",
                table: "review_recruiter",
                column: "moderator_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_recruiter_recruiter_id",
                table: "review_recruiter",
                column: "recruiter_id");

            migrationBuilder.CreateIndex(
                name: "UQ_candidate_recruiter_review",
                table: "review_recruiter",
                columns: new[] { "candidate_id", "recruiter_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__user_acc__AB6E6164EFF0C5ED",
                table: "user_account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_user_google_id",
                table: "user_account",
                column: "google_id",
                unique: true,
                filter: "([google_id] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "blog_post");

            migrationBuilder.DropTable(
                name: "bookmark");

            migrationBuilder.DropTable(
                name: "candidate_skill");

            migrationBuilder.DropTable(
                name: "interview");

            migrationBuilder.DropTable(
                name: "job_tech_stack");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "review_recruiter");

            migrationBuilder.DropTable(
                name: "application");

            migrationBuilder.DropTable(
                name: "common_technology");

            migrationBuilder.DropTable(
                name: "cv");

            migrationBuilder.DropTable(
                name: "job_post");

            migrationBuilder.DropTable(
                name: "candidate");

            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "recruiter_package_history");

            migrationBuilder.DropTable(
                name: "common_job_position");

            migrationBuilder.DropTable(
                name: "package_transaction");

            migrationBuilder.DropTable(
                name: "promotion");

            migrationBuilder.DropTable(
                name: "recruiter");

            migrationBuilder.DropTable(
                name: "service_package");

            migrationBuilder.DropTable(
                name: "user_account");
        }
    }
}
