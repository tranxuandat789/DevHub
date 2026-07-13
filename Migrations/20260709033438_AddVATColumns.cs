using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class AddVATColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.RenameColumn(
            //     name: "Recommend",
            //     table: "review_company",
            //     newName: "recommend");

            // migrationBuilder.RenameColumn(
            //     name: "WorkspaceRating",
            //     table: "review_company",
            //     newName: "workspace_rating");

            // migrationBuilder.RenameColumn(
            //     name: "TrainingRating",
            //     table: "review_company",
            //     newName: "training_rating");

            // migrationBuilder.RenameColumn(
            //     name: "SalaryRating",
            //     table: "review_company",
            //     newName: "salary_rating");

            // migrationBuilder.RenameColumn(
            //     name: "OtPolicy",
            //     table: "review_company",
            //     newName: "ot_policy");

            // migrationBuilder.RenameColumn(
            //     name: "CultureRating",
            //     table: "review_company",
            //     newName: "culture_rating");

            // migrationBuilder.RenameColumn(
            //     name: "CareRating",
            //     table: "review_company",
            //     newName: "care_rating");

            migrationBuilder.AlterColumn<string>(
                name: "ot_policy",
                table: "review_company",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // migrationBuilder.AddColumn<bool>(
            //     name: "is_active",
            //     table: "province",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "buyer_tax_code",
                table: "package_transaction",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount",
                table: "package_transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_amount",
                table: "package_transaction",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "vat_rate",
                table: "package_transaction",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 8m);

            migrationBuilder.AddColumn<int>(
                name: "moderator_id",
                table: "company",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "moderator_task_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    moderator_id = table.Column<int>(type: "int", nullable: false),
                    task_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    assigned_by = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moderator_task_type", x => x.id);
                    table.ForeignKey(
                        name: "FK__mod_task_type__assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "admin",
                        principalColumn: "admin_id");
                    table.ForeignKey(
                        name: "FK__mod_task_type__moderator",
                        column: x => x.moderator_id,
                        principalTable: "admin",
                        principalColumn: "admin_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_company_moderator_id",
                table: "company",
                column: "moderator_id");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_task_type_assigned_by",
                table: "moderator_task_type",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_moderator_task_type_moderator_id",
                table: "moderator_task_type",
                column: "moderator_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK__company__moderator_id",
                table: "company",
                column: "moderator_id",
                principalTable: "admin",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__company__moderator_id",
                table: "company");

            migrationBuilder.DropTable(
                name: "moderator_task_type");

            migrationBuilder.DropIndex(
                name: "IX_company_moderator_id",
                table: "company");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "province");

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

            migrationBuilder.DropColumn(
                name: "moderator_id",
                table: "company");

            migrationBuilder.RenameColumn(
                name: "recommend",
                table: "review_company",
                newName: "Recommend");

            migrationBuilder.RenameColumn(
                name: "workspace_rating",
                table: "review_company",
                newName: "WorkspaceRating");

            migrationBuilder.RenameColumn(
                name: "training_rating",
                table: "review_company",
                newName: "TrainingRating");

            migrationBuilder.RenameColumn(
                name: "salary_rating",
                table: "review_company",
                newName: "SalaryRating");

            migrationBuilder.RenameColumn(
                name: "ot_policy",
                table: "review_company",
                newName: "OtPolicy");

            migrationBuilder.RenameColumn(
                name: "culture_rating",
                table: "review_company",
                newName: "CultureRating");

            migrationBuilder.RenameColumn(
                name: "care_rating",
                table: "review_company",
                newName: "CareRating");

            migrationBuilder.AlterColumn<string>(
                name: "OtPolicy",
                table: "review_company",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
