using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiftwalkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class changesfortestruns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Accounts_UserId",
                table: "Runs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Runs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Accounts_UserId",
                table: "Runs",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Accounts_UserId",
                table: "Runs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Runs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Accounts_UserId",
                table: "Runs",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
