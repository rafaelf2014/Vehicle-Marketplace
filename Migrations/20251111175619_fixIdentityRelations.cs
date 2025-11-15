using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
    public partial class fixIdentityRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Vendedor_IdVendedorNavigationIdUtilizador",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_IdVendedorNavigationIdUtilizador",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "IdVendedorNavigationIdUtilizador",
                table: "Veiculo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_ID_Vendedor",
                table: "Veiculo",
                column: "ID_Vendedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_AspNetUsers",
                table: "Veiculo",
                column: "ID_Vendedor",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_AspNetUsers",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_ID_Vendedor",
                table: "Veiculo");

            migrationBuilder.AddColumn<string>(
                name: "IdVendedorNavigationIdUtilizador",
                table: "Veiculo",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_IdVendedorNavigationIdUtilizador",
                table: "Veiculo",
                column: "IdVendedorNavigationIdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Vendedor_IdVendedorNavigationIdUtilizador",
                table: "Veiculo",
                column: "IdVendedorNavigationIdUtilizador",
                principalTable: "Vendedor",
                principalColumn: "ID_Utilizador",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
