using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUI.Migrations
{
    public partial class AddLetterLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ShortUrlActivity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShortUrlId = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    IPAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrlActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OriginalUrl = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    LetterLink = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.Id);
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
                name: "AlbaUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdInAlba = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_AlbaUsers", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_User_Account",
                        column: x => x.AccountId,
                        principalTable: "AlbaAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbaUsers");

            migrationBuilder.DropTable(
                name: "ShortUrlActivity");

            migrationBuilder.DropTable(
                name: "ShortUrls");

            migrationBuilder.DropTable(
                name: "TerritoryUserAlbaAccountLink");

            migrationBuilder.DropTable(
                name: "AlbaAccounts");

            migrationBuilder.DropTable(
                name: "TerritoryUser");
        }
    }
}
