using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TerritoryTools.Web.Data.Migrations
{
    public partial class SeedTerritoryUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TerritoryUser",
                columns: new[] { "Id", "Alias", "AspNetUserId", "Created", "Email", "GivenName", "LastSignIn", "Role", "Surname", "Telephone", "Updated" },
                values: new object[] { new Guid("0714316c-8a94-438d-9f76-4c4c9b77ef89"), null, null, new DateTime(2021, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@territorytools.org", "Admin", null, "Administrator", null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TerritoryUser",
                keyColumn: "Id",
                keyValue: new Guid("0714316c-8a94-438d-9f76-4c4c9b77ef89"));
        }
    }
}
