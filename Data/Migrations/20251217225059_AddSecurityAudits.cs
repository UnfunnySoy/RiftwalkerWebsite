using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiftwalkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityAudits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Accounts",
                newName: "DeviceId");

            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccountId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SaveData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSaves_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityAudits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_AccountId",
                table: "GameSaves",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSaves");

            migrationBuilder.DropTable(
                name: "SecurityAudits");

            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Accounts",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
