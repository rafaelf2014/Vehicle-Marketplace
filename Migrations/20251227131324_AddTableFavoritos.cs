using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class AddTableFavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponível",
                table: "Veiculo");

            migrationBuilder.CreateTable(
                name: "Favorito",
                columns: table => new
                {
                    ID_Favorito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorito", x => x.ID_Favorito);
                    table.ForeignKey(
                        name: "FK_Favorito_Anuncio",
                        column: x => x.ID_Anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "ID_Anuncio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorito_AspNetUsers",
                        column: x => x.ID_Utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_ID_Anuncio",
                table: "Favorito",
                column: "ID_Anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_User_Anuncio",
                table: "Favorito",
                columns: new[] { "ID_Utilizador", "ID_Anuncio" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorito");

            migrationBuilder.AddColumn<bool>(
                name: "Disponível",
                table: "Veiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
