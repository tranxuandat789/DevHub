using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class AddModeratorIdToJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'job_post' AND COLUMN_NAME = 'moderator_id'
                )
                BEGIN
                    ALTER TABLE job_post ADD moderator_id INT NULL;
                END
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'company' AND COLUMN_NAME = 'moderator_id'
                )
                BEGIN
                    ALTER TABLE company ADD moderator_id INT NULL;
                END
            ");

            // Check if moderator_task_type exists, if not, create it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[moderator_task_type]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[moderator_task_type](
                        [id] [int] IDENTITY(1,1) NOT NULL,
                        [moderator_id] [int] NOT NULL,
                        [task_type] [nvarchar](30) NOT NULL,
                        [assigned_by] [int] NOT NULL,
                        [created_at] [datetime] NULL DEFAULT (getdate()),
                        [updated_at] [datetime] NULL DEFAULT (getdate()),
                     CONSTRAINT [PK_moderator_task_type] PRIMARY KEY CLUSTERED 
                    (
                        [id] ASC
                    )
                    ) ON [PRIMARY]
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
