using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class addMarcaFKVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Condicao",
                table: "Veiculo",
                type: "char(1)",
                unicode: false,
                fixedLength: true,
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(1)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdMarca",
                table: "Veiculo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdMarcaNavigationIdMarca",
                table: "Veiculo",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Imagem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ID_Vendedor",
                table: "Anuncio",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Marca_IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "IdMarca",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "IdMarcaNavigationIdMarca",
                table: "Veiculo");

            migrationBuilder.AlterColumn<string>(
                name: "Condicao",
                table: "Veiculo",
                type: "char(1)",
                unicode: false,
                fixedLength: true,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(1)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 1);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Imagem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ID_Vendedor",
                table: "Anuncio",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
