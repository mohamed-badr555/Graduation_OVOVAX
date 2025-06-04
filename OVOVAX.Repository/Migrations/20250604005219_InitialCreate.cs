using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InjectionSessions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RangeOfInfrared = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    StepOfInjection = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    VolumeOfLiquid = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    NumberOfElements = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjectionSessions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MovementCommands",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Axis = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementCommands", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ScanResults",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepthMeasurement = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanResults", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InjectionRecords",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InjectionSessionId = table.Column<int>(type: "int", nullable: false),
                    InjectionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EggNumber = table.Column<int>(type: "int", nullable: false),
                    VolumeInjected = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjectionRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InjectionRecords_InjectionSessions_InjectionSessionId",
                        column: x => x.InjectionSessionId,
                        principalTable: "InjectionSessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InjectionRecords_InjectionSessionId",
                table: "InjectionRecords",
                column: "InjectionSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InjectionRecords");

            migrationBuilder.DropTable(
                name: "MovementCommands");

            migrationBuilder.DropTable(
                name: "ScanResults");

            migrationBuilder.DropTable(
                name: "InjectionSessions");
        }
    }
}
