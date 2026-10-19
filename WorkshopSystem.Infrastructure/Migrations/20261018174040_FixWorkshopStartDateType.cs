using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkshopStartDateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshops_Categories_CategoryId",
                table: "Workshops");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Workshops_Categories_CategoryId",
                table: "Workshops",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workshops_Categories_CategoryId",
                table: "Workshops");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Workshops_Categories_CategoryId",
                table: "Workshops",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
