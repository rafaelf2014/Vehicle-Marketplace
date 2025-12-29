using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTableVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Venda",
                columns: table => new
                {
                    IdVenda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAnuncio = table.Column<int>(type: "int", nullable: false),
                    IdComprador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrecoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venda", x => x.IdVenda);
                    table.ForeignKey(
                        name: "FK_Venda_Anuncio_IdAnuncio",
                        column: x => x.IdAnuncio,
                        principalTable: "Anuncio",
                        principalColumn: "ID_Anuncio");
                    table.ForeignKey(
                        name: "FK_Venda_AspNetUsers_IdComprador",
                        column: x => x.IdComprador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Venda_IdAnuncio",
                table: "Venda",
                column: "IdAnuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Venda_IdComprador",
                table: "Venda",
                column: "IdComprador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Venda");
        }
    }
}
