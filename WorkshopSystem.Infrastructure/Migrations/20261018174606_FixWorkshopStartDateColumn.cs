using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkshopStartDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "Workshops"
        ALTER COLUMN "StartDate"
        TYPE date
        USING "StartDate"::date;
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
        ALTER TABLE "Workshops"
        ALTER COLUMN "StartDate"
        TYPE timestamp with time zone
        USING "StartDate"::timestamp with time zone;
        """);
        }
    }
}
