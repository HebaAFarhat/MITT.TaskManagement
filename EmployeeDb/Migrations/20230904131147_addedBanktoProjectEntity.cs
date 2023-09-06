using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITT.EmployeeDb.Migrations
{
    /// <inheritdoc />
    public partial class addedBanktoProjectEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bank",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Bank_ProjectType",
                table: "Projects",
                columns: new[] { "Bank", "ProjectType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Bank_ProjectType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "Projects");
        }
    }
}
