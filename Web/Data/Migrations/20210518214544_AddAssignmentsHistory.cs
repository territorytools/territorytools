using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TerritoryTools.Web.Data.Migrations
{
    public partial class AddAssignmentsHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TerritoryAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TerritoryNumber = table.Column<string>(nullable: true),
                    PublisherName = table.Column<string>(nullable: true),
                    CheckedOut = table.Column<DateTime>(nullable: true),
                    CheckedIn = table.Column<DateTime>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedByUser = table.Column<Guid>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedByUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritoryAssignments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerritoryAssignments");
        }
    }
}
