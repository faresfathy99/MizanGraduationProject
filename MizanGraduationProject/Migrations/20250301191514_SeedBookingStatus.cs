using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MizanGraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BookingStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { "12e7996f-58dc-4fa0-a840-ec6c6dfa4ca0", "Rejected" },
                    { "708c1175-03be-4730-8acc-0fb29159a3ce", "Approved" },
                    { "d48d2a28-c7b1-4a24-be63-634d2d9a4a1f", "Pending" },
                    { "ebd1fc49-4e1a-4e3d-b23f-c45656baf1e9", "Completed" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: "12e7996f-58dc-4fa0-a840-ec6c6dfa4ca0");

            migrationBuilder.DeleteData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: "708c1175-03be-4730-8acc-0fb29159a3ce");

            migrationBuilder.DeleteData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: "d48d2a28-c7b1-4a24-be63-634d2d9a4a1f");

            migrationBuilder.DeleteData(
                table: "BookingStatuses",
                keyColumn: "Id",
                keyValue: "ebd1fc49-4e1a-4e3d-b23f-c45656baf1e9");
        }
    }
}
