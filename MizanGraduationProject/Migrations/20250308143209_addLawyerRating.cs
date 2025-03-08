using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MizanGraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class addLawyerRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Rate",
                table: "Lawyers",
                type: "smallint",
                maxLength: 10,
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Lawyers");
        }
    }
}
