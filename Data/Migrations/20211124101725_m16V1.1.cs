using Microsoft.EntityFrameworkCore.Migrations;

namespace Ataa.Data.Migrations
{
    public partial class m16V11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClockCoin",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockCoin",
                table: "AspNetUsers");
        }
    }
}
