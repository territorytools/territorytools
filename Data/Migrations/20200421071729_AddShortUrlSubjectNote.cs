using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Migrations
{
    public partial class AddShortUrlSubjectNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "Subject",
               table: "ShortUrls",
               nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "Note",
              table: "ShortUrls",
              nullable: true);

            migrationBuilder.CreateTable(
                name: "AlbaUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastSignIn = table.Column<DateTime>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbaUser", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbaUser");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ShortUrls");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "ShortUrls");
        }
    }
}
