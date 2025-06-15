using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InjectionOperations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InjectionOperations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
