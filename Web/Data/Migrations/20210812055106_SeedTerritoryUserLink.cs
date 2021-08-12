using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TerritoryTools.Web.Data.Migrations
{
    public partial class SeedTerritoryUserLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TerritoryUserAlbaAccountLinkId",
                table: "TerritoryUserAlbaAccountLink",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrls",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrlHosts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrlActivity",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.InsertData(
                table: "AlbaAccounts",
                columns: new[] { "Id", "AccountName", "Created", "HostName", "IdInAlba", "LongName", "Updated" },
                values: new object[] { new Guid("90bc0598-1907-4384-8d8d-e5d336c769c3"), "account-name", new DateTime(2021, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "host-name", 1, "Long Name for Alba Account", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "TerritoryUserAlbaAccountLink",
                columns: new[] { "TerritoryUserAlbaAccountLinkId", "AlbaAccountId", "Created", "Role", "TerritoryUserId", "Updated" },
                values: new object[] { 1, new Guid("90bc0598-1907-4384-8d8d-e5d336c769c3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("0714316c-8a94-438d-9f76-4c4c9b77ef89"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TerritoryUserAlbaAccountLink",
                keyColumn: "TerritoryUserAlbaAccountLinkId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AlbaAccounts",
                keyColumn: "Id",
                keyValue: new Guid("90bc0598-1907-4384-8d8d-e5d336c769c3"));

            migrationBuilder.AlterColumn<int>(
                name: "TerritoryUserAlbaAccountLinkId",
                table: "TerritoryUserAlbaAccountLink",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrls",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrlHosts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ShortUrlActivity",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
