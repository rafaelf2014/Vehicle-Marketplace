using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class RecoverMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "Disponível",
                table: "Veiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponível",
                table: "Veiculo");

            migrationBuilder.AddColumn<int>(
                name: "NVisitas",
                table: "Anuncio",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
