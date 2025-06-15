using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OVOVAX.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScanResultToSensorReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepthMeasurement",
                table: "ScanResults");

            migrationBuilder.AddColumn<string>(
                name: "SensorReadings",
                table: "ScanResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SensorReadings",
                table: "ScanResults");

            migrationBuilder.AddColumn<double>(
                name: "DepthMeasurement",
                table: "ScanResults",
                type: "float(18)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
