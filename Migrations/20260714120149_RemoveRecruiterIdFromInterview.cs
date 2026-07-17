using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecruiterIdFromInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__interview__recru__7D439ABD",
                table: "interview");

            migrationBuilder.DropIndex(
                name: "IX_interview_recruiter_id",
                table: "interview");

            migrationBuilder.DropColumn(
                name: "recruiter_id",
                table: "interview");

            // Duplicate columns removed: buyer_tax_code, total_amount, vat_amount, vat_rate

            migrationBuilder.CreateTable(
                name: "moderator_industry_assignment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    moderator_id = table.Column<int>(type: "int", nullable: false),
                    task_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    assigned_by = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderator_industry_assignment", x => x.id);
                    table.ForeignKey(
                        name: "FK_mod_industry_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                    table.ForeignKey(
                        name: "FK_mod_industry_moderator",
                        column: x => x.moderator_id,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_mod_industry_task_industry",
                table: "moderator_industry_assignment",
                columns: new[] { "task_type", "industry" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_moderator_industry_assignment_assigned_by",
                table: "moderator_industry_assignment",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_industry_assignment_moderator_id",
                table: "moderator_industry_assignment",
                column: "moderator_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "moderator_industry_assignment");

            migrationBuilder.DropColumn(
                name: "buyer_tax_code",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "total_amount",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "vat_amount",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "vat_rate",
                table: "package_transaction");

            migrationBuilder.AddColumn<int>(
                name: "recruiter_id",
                table: "interview",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_interview_recruiter_id",
                table: "interview",
                column: "recruiter_id");

            migrationBuilder.AddForeignKey(
                name: "FK__interview__recru__7D439ABD",
                table: "interview",
                column: "recruiter_id",
                principalTable: "recruiter",
                principalColumn: "recruiter_id");
        }
    }
}
