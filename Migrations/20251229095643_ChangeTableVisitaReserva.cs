using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableVisitaReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__VisitaRes__ID_An__6FE99F9F",
                table: "VisitaReserva");

            migrationBuilder.AddColumn<int>(
                name: "AnuncioIdAnuncio",
                table: "VisitaReserva",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitaReserva_AnuncioIdAnuncio",
                table: "VisitaReserva",
                column: "AnuncioIdAnuncio");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaReserva_ID_Comprador",
                table: "VisitaReserva",
                column: "ID_Comprador");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaReserva_Anuncio_AnuncioIdAnuncio",
                table: "VisitaReserva",
                column: "AnuncioIdAnuncio",
                principalTable: "Anuncio",
                principalColumn: "ID_Anuncio");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaReserva_Anuncio_ID_Anuncio",
                table: "VisitaReserva",
                column: "ID_Anuncio",
                principalTable: "Anuncio",
                principalColumn: "ID_Anuncio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitaReserva_AspNetUsers_ID_Comprador",
                table: "VisitaReserva",
                column: "ID_Comprador",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitaReserva_Anuncio_AnuncioIdAnuncio",
                table: "VisitaReserva");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaReserva_Anuncio_ID_Anuncio",
                table: "VisitaReserva");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitaReserva_AspNetUsers_ID_Comprador",
                table: "VisitaReserva");

            migrationBuilder.DropIndex(
                name: "IX_VisitaReserva_AnuncioIdAnuncio",
                table: "VisitaReserva");

            migrationBuilder.DropIndex(
                name: "IX_VisitaReserva_ID_Comprador",
                table: "VisitaReserva");

            migrationBuilder.DropColumn(
                name: "AnuncioIdAnuncio",
                table: "VisitaReserva");

            migrationBuilder.AddForeignKey(
                name: "FK__VisitaRes__ID_An__6FE99F9F",
                table: "VisitaReserva",
                column: "ID_Anuncio",
                principalTable: "Anuncio",
                principalColumn: "ID_Anuncio");
        }
    }
}
