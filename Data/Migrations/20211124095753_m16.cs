using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ataa.Data.Migrations
{
    public partial class m16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JodId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<String>(
               name: "Location",
               table: "RequestForm",
               nullable: false,
               defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "RequestForm");
                                

         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
