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
            migrationBuilder.Sql(@"
                DECLARE @ConstraintName nvarchar(200);
                SELECT @ConstraintName = name
                FROM sys.key_constraints
                WHERE type = 'UQ' AND parent_object_id = OBJECT_ID('blog_post');

                IF @ConstraintName IS NOT NULL
                BEGIN
                    EXEC('ALTER TABLE blog_post DROP CONSTRAINT ' + @ConstraintName);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE blog_post ADD CONSTRAINT UQ__blog_pos__DC101C013917481F UNIQUE (Tag)");
        }
    }
}
