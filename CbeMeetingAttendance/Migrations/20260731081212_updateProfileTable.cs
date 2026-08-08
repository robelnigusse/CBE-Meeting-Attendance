using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CbeMeetingAttendance.Migrations
{
    /// <inheritdoc />
    public partial class updateProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "path",
                table: "Profiles",
                newName: "Path");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Profiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Profiles",
                newName: "path");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles",
                column: "EmployeeId");
        }
    }
}
