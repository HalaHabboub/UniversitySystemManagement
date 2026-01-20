using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystemManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentCards",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCards", x => x.StudentId);
                    table.ForeignKey(
                        name: "FK_StudentCards_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCards");
        }
    }
}
