using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class MakePublisherNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__blog_post__publi__395884C4",
                table: "blog_post");

            migrationBuilder.AlterColumn<int>(
                name: "publisher_id",
                table: "blog_post",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK__blog_post__publi__395884C4",
                table: "blog_post",
                column: "publisher_id",
                principalTable: "admin",
                principalColumn: "admin_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__blog_post__publi__395884C4",
                table: "blog_post");

            migrationBuilder.AlterColumn<int>(
                name: "publisher_id",
                table: "blog_post",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__blog_post__publi__395884C4",
                table: "blog_post",
                column: "publisher_id",
                principalTable: "admin",
                principalColumn: "admin_id");
        }
    }
}
