using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MITT.EmployeeDb.Migrations
{
    /// <inheritdoc />
    public partial class secondMangmentDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedBETasks_Developers_DeveloperId",
                table: "AssignedBETasks");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedManagers_Managers_ProjectManagerId",
                table: "AssignedManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedQATasks_Developers_DeveloperId",
                table: "AssignedQATasks");

            migrationBuilder.DropTable(
                name: "Developers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Managers",
                table: "Managers");

            migrationBuilder.RenameTable(
                name: "Managers",
                newName: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeType",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedBETasks_Employees_DeveloperId",
                table: "AssignedBETasks",
                column: "DeveloperId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedManagers_Employees_ProjectManagerId",
                table: "AssignedManagers",
                column: "ProjectManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedQATasks_Employees_DeveloperId",
                table: "AssignedQATasks",
                column: "DeveloperId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedBETasks_Employees_DeveloperId",
                table: "AssignedBETasks");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedManagers_Employees_ProjectManagerId",
                table: "AssignedManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedQATasks_Employees_DeveloperId",
                table: "AssignedQATasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Managers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Managers",
                table: "Managers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActiveState = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedBETasks_Developers_DeveloperId",
                table: "AssignedBETasks",
                column: "DeveloperId",
                principalTable: "Developers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedManagers_Managers_ProjectManagerId",
                table: "AssignedManagers",
                column: "ProjectManagerId",
                principalTable: "Managers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedQATasks_Developers_DeveloperId",
                table: "AssignedQATasks",
                column: "DeveloperId",
                principalTable: "Developers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
