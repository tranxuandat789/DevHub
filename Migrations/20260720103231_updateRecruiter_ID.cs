using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHub.Migrations
{
    /// <inheritdoc />
    public partial class updateRecruiter_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
// migrationBuilder.AddColumn<int>(
//     name: "recruiter_id",
//     table: "package_transaction",
//     type: "int",
//     nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recruiter_id",
                table: "interview",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_package_transaction_recruiter_id",
                table: "package_transaction",
                column: "recruiter_id");

// migrationBuilder.AddForeignKey(
//     name: "FK_package_transaction_recruiter",
//     table: "package_transaction",
//     column: "recruiter_id",
//     principalTable: "recruiter",
//     principalColumn: "recruiter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_package_transaction_recruiter",
                table: "package_transaction");

            migrationBuilder.DropIndex(
                name: "IX_package_transaction_recruiter_id",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "recruiter_id",
                table: "package_transaction");

            migrationBuilder.DropColumn(
                name: "recruiter_id",
                table: "interview");
        }
    }
}
