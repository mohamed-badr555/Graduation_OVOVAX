using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStepFromMovementCommand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Step",
                table: "MovementCommands");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Step",
                table: "MovementCommands",
                type: "float(18)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
