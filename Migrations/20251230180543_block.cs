using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class block : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "SitePageView");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "SitePageView");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitTime",
                table: "SitePageView",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "SitePageView",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "UserBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId",
                table: "UserBlocks",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBlocks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VisitTime",
                table: "SitePageView",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "SitePageView",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "SitePageView",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "SitePageView",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
