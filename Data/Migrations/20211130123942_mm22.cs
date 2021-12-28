using Microsoft.EntityFrameworkCore.Migrations;

namespace Ataa.Data.Migrations
{
    public partial class mm22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "AttendanceList",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FormId = table.Column<int>(nullable: false),
                UserId = table.Column<string>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AttendanceList", x => x.Id);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
