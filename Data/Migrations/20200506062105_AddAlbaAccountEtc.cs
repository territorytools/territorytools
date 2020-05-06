using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Migrations
{
    public partial class AddAlbaAccountEtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbaUser",
                table: "AlbaUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AlbaUser");

            migrationBuilder.RenameTable(
                name: "AlbaUser",
                newName: "AlbaUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AlbaUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "AlbaUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "IdInAlba",
                table: "AlbaUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbaUsers",
                table: "AlbaUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AlbaAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdInAlba = table.Column<int>(nullable: false),
                    AccountName = table.Column<string>(nullable: true),
                    LongName = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbaAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerritoryUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Alias = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    GivenName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    AspNetUserId = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastSignIn = table.Column<DateTime>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritoryUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerritoryUserAlbaAccountLink",
                columns: table => new
                {
                    TerritoryUserAlbaAccountLinkId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TerritoryUserId = table.Column<Guid>(nullable: false),
                    AlbaAccountId = table.Column<Guid>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerritoryUserAlbaAccountLink", x => x.TerritoryUserAlbaAccountLinkId);
                    table.ForeignKey(
                        name: "ForeignKey_AlbaAccount_TerritoryUser_Link",
                        column: x => x.AlbaAccountId,
                        principalTable: "AlbaAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_TerritoryUser_AlbaAccount_Link",
                        column: x => x.TerritoryUserId,
                        principalTable: "TerritoryUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbaUsers_AccountId",
                table: "AlbaUsers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TerritoryUserAlbaAccountLink_AlbaAccountId",
                table: "TerritoryUserAlbaAccountLink",
                column: "AlbaAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TerritoryUserAlbaAccountLink_TerritoryUserId",
                table: "TerritoryUserAlbaAccountLink",
                column: "TerritoryUserId");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_User_Account",
                table: "AlbaUsers",
                column: "AccountId",
                principalTable: "AlbaAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Account",
                table: "AlbaUsers");

            migrationBuilder.DropTable(
                name: "TerritoryUserAlbaAccountLink");

            migrationBuilder.DropTable(
                name: "AlbaAccounts");

            migrationBuilder.DropTable(
                name: "TerritoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbaUsers",
                table: "AlbaUsers");

            migrationBuilder.DropIndex(
                name: "IX_AlbaUsers_AccountId",
                table: "AlbaUsers");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AlbaUsers");

            migrationBuilder.DropColumn(
                name: "IdInAlba",
                table: "AlbaUsers");

            migrationBuilder.RenameTable(
                name: "AlbaUsers",
                newName: "AlbaUser");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AlbaUser",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbaUser",
                table: "AlbaUser",
                column: "Id");
        }
    }
}
