using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class AddDisponivelToVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Marca_IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.AddColumn<string>(
                name: "Caixa",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Disponivel",
                table: "Veiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_IdMarca",
                table: "Veiculo",
                column: "IdMarca");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_ID_Vendedor",
                table: "Anuncio",
                column: "ID_Vendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Anuncio_AspNetUsers",
                table: "Anuncio",
                column: "ID_Vendedor",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Marca",
                table: "Veiculo",
                column: "IdMarca",
                principalTable: "Marca",
                principalColumn: "ID_Marca");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anuncio_AspNetUsers",
                table: "Anuncio");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Marca",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_IdMarca",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Anuncio_ID_Vendedor",
                table: "Anuncio");

            migrationBuilder.DropColumn(
                name: "Caixa",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "Disponivel",
                table: "Veiculo");

            migrationBuilder.AddColumn<int>(
                name: "IdMarcaNavigationIdMarca",
                table: "Veiculo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_IdMarcaNavigationIdMarca",
                table: "Veiculo",
                column: "IdMarcaNavigationIdMarca");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Marca_IdMarcaNavigationIdMarca",
                table: "Veiculo",
                column: "IdMarcaNavigationIdMarca",
                principalTable: "Marca",
                principalColumn: "ID_Marca");
        }
    }
}
