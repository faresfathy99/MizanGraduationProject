using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MizanGraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class add_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTypeModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypeModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTypeModels_Name",
                table: "UserTypeModels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTypeModels_NormalizedName",
                table: "UserTypeModels",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTypeModels");
        }
    }
}
