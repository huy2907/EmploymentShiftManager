using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmploymentShiftManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Employees",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Employees",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Employees",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Employees",
                newName: "FullName");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
