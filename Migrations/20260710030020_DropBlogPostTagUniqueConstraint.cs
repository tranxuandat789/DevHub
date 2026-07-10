using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class DropBlogPostTagUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE blog_post DROP CONSTRAINT UQ__blog_pos__DC101C013917481F");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE blog_post ADD CONSTRAINT UQ__blog_pos__DC101C013917481F UNIQUE (Tag)");
        }
    }
}
