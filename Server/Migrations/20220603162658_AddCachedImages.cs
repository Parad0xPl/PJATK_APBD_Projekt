using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    public partial class AddCachedImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cached",
                columns: table => new
                {
                    Url = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    Data = table.Column<byte[]>(type: "BLOB", maxLength: 4194304, nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cached", x => x.Url);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cached");
        }
    }
}
