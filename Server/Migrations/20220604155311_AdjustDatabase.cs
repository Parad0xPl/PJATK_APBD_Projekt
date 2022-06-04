using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    public partial class AdjustDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Cached",
                newName: "CachedImages"
            );
            // migrationBuilder.DropTable(
            //     name: "Cached");

            migrationBuilder.RenameColumn(
                name: "RequestJSON",
                table: "Stocks",
                newName: "RequestJson");

            // migrationBuilder.CreateTable(
            //     name: "CachedImages",
            //     columns: table => new
            //     {
            //         Url = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
            //         Data = table.Column<byte[]>(type: "BLOB", maxLength: 4194304, nullable: false),
            //         Type = table.Column<string>(type: "TEXT", nullable: false),
            //         CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_CachedImages", x => x.Url);
            //     });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Ticker",
                table: "Stocks",
                column: "Ticker",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CachedImages",
                newName: "Cached"
            );
            // migrationBuilder.DropTable(
            //     name: "CachedImages");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_Ticker",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "RequestJson",
                table: "Stocks",
                newName: "RequestJSON");

            // migrationBuilder.CreateTable(
            //     name: "Cached",
            //     columns: table => new
            //     {
            //         Url = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
            //         CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
            //         Data = table.Column<byte[]>(type: "BLOB", maxLength: 4194304, nullable: false),
            //         Type = table.Column<string>(type: "TEXT", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Cached", x => x.Url);
            //     });
        }
    }
}
