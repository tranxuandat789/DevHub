using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorNameToBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_article_admin_AdminId",
                table: "article");

            migrationBuilder.DropIndex(
                name: "IX_article_AdminId",
                table: "article");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "article");

            migrationBuilder.AddColumn<string>(
                name: "interview_type",
                table: "interview",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "author_name",
                table: "blog_post",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "interview_type",
                table: "interview");

            migrationBuilder.DropColumn(
                name: "author_name",
                table: "blog_post");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "article",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_article_AdminId",
                table: "article",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_article_admin_AdminId",
                table: "article",
                column: "AdminId",
                principalTable: "admin",
                principalColumn: "admin_id");
        }
    }
}
