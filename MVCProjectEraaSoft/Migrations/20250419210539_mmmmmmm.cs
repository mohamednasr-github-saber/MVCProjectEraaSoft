using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCProjectEraaSoft.Migrations
{
    /// <inheritdoc />
    public partial class mmmmmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "Appointments");
        }
    }
}
