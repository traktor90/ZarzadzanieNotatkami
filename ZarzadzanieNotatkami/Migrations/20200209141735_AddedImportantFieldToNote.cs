using Microsoft.EntityFrameworkCore.Migrations;

namespace ZarzadzanieNotatkami.Migrations
{
    public partial class AddedImportantFieldToNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Important",
                table: "Notes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Important",
                table: "Notes");
        }
    }
}
