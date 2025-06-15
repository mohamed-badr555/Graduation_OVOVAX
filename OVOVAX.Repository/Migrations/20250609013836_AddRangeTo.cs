using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddRangeTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RangeOfInfrared",
                table: "InjectionSessions",
                newName: "RangeOfInfraredTo");

            migrationBuilder.AddColumn<double>(
                name: "RangeOfInfraredFrom",
                table: "InjectionSessions",
                type: "float(18)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RangeOfInfraredFrom",
                table: "InjectionSessions");

            migrationBuilder.RenameColumn(
                name: "RangeOfInfraredTo",
                table: "InjectionSessions",
                newName: "RangeOfInfrared");
        }
    }
}
