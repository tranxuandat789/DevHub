using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Already applied via Add_ReviewRatingDetails.sql
            /*
            migrationBuilder.AddColumn<int>(
                name: "CareRating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CultureRating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtPolicy",
                table: "review_company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Recommend",
                table: "review_company",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalaryRating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingRating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkspaceRating",
                table: "review_company",
                type: "int",
                nullable: true);
            */

            // migrationBuilder.AddColumn<string>(
            //     name: "interview_type",
            //     table: "interview",
            //     type: "nvarchar(50)",
            //     maxLength: 50,
            //     nullable: true);

            // migrationBuilder.AddColumn<int>(
            //     name: "company_id",
            //     table: "job_post",
            //     type: "int",
            //     nullable: false,
            //     defaultValue: 1);

            // migrationBuilder.AddColumn<int>(
            //     name: "company_package_history_id",
            //     table: "job_post",
            //     type: "int",
            //     nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CareRating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "CultureRating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "OtPolicy",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "Recommend",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "SalaryRating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "TrainingRating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "WorkspaceRating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "interview_type",
                table: "interview");
        }
    }
}
