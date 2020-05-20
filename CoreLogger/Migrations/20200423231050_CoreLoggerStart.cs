using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreLogger.Migrations
{
    public partial class CoreLoggerStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Log");

            migrationBuilder.CreateTable(
                name: "Log_Master",
                schema: "Log",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallerMemberName = table.Column<string>(nullable: true),
                    CallerMemberLineNumber = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    LevelID = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    FullData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_Master", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log_Master",
                schema: "Log");
        }
    }
}
