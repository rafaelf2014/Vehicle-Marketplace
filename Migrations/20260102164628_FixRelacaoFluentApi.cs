using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacaoFluentApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitaReserva_Anuncio_AnuncioIdAnuncio",
                table: "VisitaReserva");

            migrationBuilder.DropIndex(
                name: "IX_VisitaReserva_AnuncioIdAnuncio",
                table: "VisitaReserva");

            migrationBuilder.DropColumn(
                name: "AnuncioIdAnuncio",
                table: "VisitaReserva");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnuncioIdAnuncio",
                table: "VisitaReserva",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitaReserva_AnuncioIdAnuncio",
                table: "VisitaReserva",
                column: "AnuncioIdAnuncio");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaReserva_Anuncio_AnuncioIdAnuncio",
                table: "VisitaReserva",
                column: "AnuncioIdAnuncio",
                principalTable: "Anuncio",
                principalColumn: "ID_Anuncio");
        }
    }
}
