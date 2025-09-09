using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmploymentShiftManager.Migrations
{
    /// <inheritdoc />
    public partial class AddColorAndRoleToShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Shifts",
                newName: "Color");

            migrationBuilder.AddColumn<string>(
                name: "AssignedRole",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedRole",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Shifts",
                newName: "Description");
        }
    }
}
