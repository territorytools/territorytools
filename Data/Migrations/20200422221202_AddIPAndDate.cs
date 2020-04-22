using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Migrations
{
    public partial class AddIPAndDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ShortUrls",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "ShortUrlActivity",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "ShortUrls");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "ShortUrlActivity");
        }
    }
}
