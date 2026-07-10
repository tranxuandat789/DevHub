using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewCompanyRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "salary_rating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "training_rating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "care_rating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "culture_rating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "workspace_rating",
                table: "review_company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ot_policy",
                table: "review_company",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "recommend",
                table: "review_company",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salary_rating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "training_rating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "care_rating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "culture_rating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "workspace_rating",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "ot_policy",
                table: "review_company");

            migrationBuilder.DropColumn(
                name: "recommend",
                table: "review_company");
        }
    }
}
