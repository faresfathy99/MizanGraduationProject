using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MizanGraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class addReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lawyer_AspNetUsers_UserId",
                table: "Lawyer");

            migrationBuilder.DropForeignKey(
                name: "FK_Lawyer_Specialization_SpecializationId",
                table: "Lawyer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lawyer",
                table: "Lawyer");

            migrationBuilder.RenameTable(
                name: "Specialization",
                newName: "Specializations");

            migrationBuilder.RenameTable(
                name: "Lawyer",
                newName: "Lawyers");

            migrationBuilder.RenameIndex(
                name: "IX_Lawyer_UserId",
                table: "Lawyers",
                newName: "IX_Lawyers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Lawyer_SpecializationId",
                table: "Lawyers",
                newName: "IX_Lawyers_SpecializationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lawyers",
                table: "Lawyers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_LawyerId",
                table: "Reviews",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_AspNetUsers_UserId",
                table: "Lawyers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_Specializations_SpecializationId",
                table: "Lawyers",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_AspNetUsers_UserId",
                table: "Lawyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_Specializations_SpecializationId",
                table: "Lawyers");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lawyers",
                table: "Lawyers");

            migrationBuilder.RenameTable(
                name: "Specializations",
                newName: "Specialization");

            migrationBuilder.RenameTable(
                name: "Lawyers",
                newName: "Lawyer");

            migrationBuilder.RenameIndex(
                name: "IX_Lawyers_UserId",
                table: "Lawyer",
                newName: "IX_Lawyer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Lawyers_SpecializationId",
                table: "Lawyer",
                newName: "IX_Lawyer_SpecializationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lawyer",
                table: "Lawyer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyer_AspNetUsers_UserId",
                table: "Lawyer",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyer_Specialization_SpecializationId",
                table: "Lawyer",
                column: "SpecializationId",
                principalTable: "Specialization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
