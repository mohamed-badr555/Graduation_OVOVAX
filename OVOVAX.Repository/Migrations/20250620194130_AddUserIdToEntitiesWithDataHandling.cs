using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEntitiesWithDataHandling : Migration
    {        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear existing data to avoid foreign key constraint conflicts
            migrationBuilder.Sql("DELETE FROM [InjectionOperations]");
            migrationBuilder.Sql("DELETE FROM [ScanResults]");
            migrationBuilder.Sql("DELETE FROM [MovementCommands]");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ScanResults",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MovementCommands",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "InjectionOperations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ScanResults_UserId",
                table: "ScanResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovementCommands_UserId",
                table: "MovementCommands",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InjectionOperations_UserId",
                table: "InjectionOperations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InjectionOperations_AspNetUsers_UserId",
                table: "InjectionOperations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementCommands_AspNetUsers_UserId",
                table: "MovementCommands",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanResults_AspNetUsers_UserId",
                table: "ScanResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InjectionOperations_AspNetUsers_UserId",
                table: "InjectionOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementCommands_AspNetUsers_UserId",
                table: "MovementCommands");

            migrationBuilder.DropForeignKey(
                name: "FK_ScanResults_AspNetUsers_UserId",
                table: "ScanResults");

            migrationBuilder.DropIndex(
                name: "IX_ScanResults_UserId",
                table: "ScanResults");

            migrationBuilder.DropIndex(
                name: "IX_MovementCommands_UserId",
                table: "MovementCommands");

            migrationBuilder.DropIndex(
                name: "IX_InjectionOperations_UserId",
                table: "InjectionOperations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ScanResults");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MovementCommands");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InjectionOperations");
        }
    }
}
