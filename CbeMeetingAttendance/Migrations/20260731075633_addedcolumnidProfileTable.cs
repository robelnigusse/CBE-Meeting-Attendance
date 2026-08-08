using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CbeMeetingAttendance.Migrations
{
    /// <inheritdoc />
    public partial class addedcolumnidProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Employees_EmployeeId1",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_EmployeeId1",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "EmployeeId1",
                table: "Profiles",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Employees_EmployeeId",
                table: "Profiles",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Employees_EmployeeId",
                table: "Profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_EmployeeId",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Profiles",
                newName: "EmployeeId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profiles",
                table: "Profiles",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_EmployeeId1",
                table: "Profiles",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Employees_EmployeeId1",
                table: "Profiles",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
