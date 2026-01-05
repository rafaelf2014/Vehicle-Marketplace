using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CliCarProject.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:Migrations/20260103103817_setupfinal.cs
    public partial class setupfinal : Migration
========
    public partial class final : Migration
>>>>>>>> origin/SilvaLukas-Branch:Migrations/20260103110645_final.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classe",
                columns: table => new
                {
                    ID_Classe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Classe__7C4BBB54FAA85776", x => x.ID_Classe);
                });

            migrationBuilder.CreateTable(
                name: "Combustivel",
                columns: table => new
                {
                    ID_Combustivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Combusti__F8AAF41E4F0B740D", x => x.ID_Combustivel);
                });

            migrationBuilder.CreateTable(
                name: "Localizacao",
                columns: table => new
                {
                    ID_Localizacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Localiza__72A2EF98171EE69F", x => x.ID_Localizacao);
                });

            migrationBuilder.CreateTable(
                name: "Marca",
                columns: table => new
                {
                    ID_Marca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Marca__9B8F8DB2441923EF", x => x.ID_Marca);
                });

            migrationBuilder.CreateTable(
                name: "SitePageView",
                columns: table => new
                {
                    ID_SitePageView = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePageView", x => x.ID_SitePageView);
                });

            migrationBuilder.CreateTable(
                name: "TipoAcao",
                columns: table => new
                {
                    ID_TipoAcao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoAcao__D8411B34A8D3749C", x => x.ID_TipoAcao);
                });

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

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    ID_Utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.ID_Utilizador);
                    table.ForeignKey(
                        name: "FK_Administrador_AspNetUsers",
                        column: x => x.ID_Utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comprador",
                columns: table => new
                {
                    ID_Utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contacto = table.Column<string>(type: "char(9)", unicode: false, fixedLength: true, maxLength: 9, nullable: true),
                    CodigoPostal = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprador", x => x.ID_Utilizador);
                    table.ForeignKey(
                        name: "FK_Comprador_AspNetUsers",
                        column: x => x.ID_Utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vendedor",
                columns: table => new
                {
                    ID_Utilizador = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Contacto = table.Column<string>(type: "char(9)", unicode: false, fixedLength: true, maxLength: 9, nullable: true),
                    CodigoPostal = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: true),
                    NIF = table.Column<string>(type: "char(9)", unicode: false, fixedLength: true, maxLength: 9, nullable: true),
                    Tipo = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedor", x => x.ID_Utilizador);
                    table.ForeignKey(
                        name: "FK_Vendedor_AspNetUsers",
                        column: x => x.ID_Utilizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FiltrosFavoritos",
                columns: table => new
                {
                    ID_FiltroFavorito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Comprador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiltrosJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ID_Combustivel = table.Column<int>(type: "int", nullable: true),
                    ID_Classe = table.Column<int>(type: "int", nullable: true),
                    ID_Localizacao = table.Column<int>(type: "int", nullable: true),
                    ID_Marca = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FiltrosF__410905C7C5E79498", x => x.ID_FiltroFavorito);
                    table.ForeignKey(
                        name: "FK__FiltrosFa__ID_Cl__693CA210",
                        column: x => x.ID_Classe,
                        principalTable: "Classe",
                        principalColumn: "ID_Classe");
                    table.ForeignKey(
                        name: "FK__FiltrosFa__ID_Co__68487DD7",
                        column: x => x.ID_Combustivel,
                        principalTable: "Combustivel",
                        principalColumn: "ID_Combustivel");
                    table.ForeignKey(
                        name: "FK__FiltrosFa__ID_Lo__6A30C649",
                        column: x => x.ID_Localizacao,
                        principalTable: "Localizacao",
                        principalColumn: "ID_Localizacao");
                    table.ForeignKey(
                        name: "FK__FiltrosFa__ID_Ma__6B24EA82",
                        column: x => x.ID_Marca,
                        principalTable: "Marca",
                        principalColumn: "ID_Marca");
                });

            migrationBuilder.CreateTable(
                name: "Modelo",
                columns: table => new
                {
                    ID_Modelo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_Marca = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Modelo__813C2372542DB0DE", x => x.ID_Modelo);
                    table.ForeignKey(
                        name: "FK__Modelo__ID_Marca__5165187F",
                        column: x => x.ID_Marca,
                        principalTable: "Marca",
                        principalColumn: "ID_Marca");
                });

            migrationBuilder.CreateTable(
                name: "Acao",
                columns: table => new
                {
                    ID_Acao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_TipoAcao = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TipoAlvo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Acao__4EE45891E21367C6", x => x.ID_Acao);
                    table.ForeignKey(
                        name: "FK__Acao__ID_TipoAca__7B5B524B",
                        column: x => x.ID_TipoAcao,
                        principalTable: "TipoAcao",
                        principalColumn: "ID_TipoAcao");
                });

            migrationBuilder.CreateTable(
                name: "Veiculo",
                columns: table => new
                {
                    ID_Veiculo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Vendedor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Ano = table.Column<int>(type: "int", nullable: true),
                    Quilometragem = table.Column<int>(type: "int", nullable: true),
                    Condicao = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    Caixa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Modelo = table.Column<int>(type: "int", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    IdMarca = table.Column<int>(type: "int", nullable: false),
                    ID_Combustivel = table.Column<int>(type: "int", nullable: false),
                    ID_Classe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Veiculo__808FFECF1FB610CC", x => x.ID_Veiculo);
                    table.ForeignKey(
                        name: "FK_Veiculo_AspNetUsers",
                        column: x => x.ID_Vendedor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Veiculo_Marca",
                        column: x => x.IdMarca,
                        principalTable: "Marca",
                        principalColumn: "ID_Marca");
                    table.ForeignKey(
                        name: "FK__Veiculo__ID_Clas__59FA5E80",
                        column: x => x.ID_Classe,
                        principalTable: "Classe",
                        principalColumn: "ID_Classe");
                    table.ForeignKey(
                        name: "FK__Veiculo__ID_Comb__59063A47",
                        column: x => x.ID_Combustivel,
                        principalTable: "Combustivel",
                        principalColumn: "ID_Combustivel");
                    table.ForeignKey(
                        name: "FK__Veiculo__ID_Mode__5812160E",
                        column: x => x.ID_Modelo,
                        principalTable: "Modelo",
                        principalColumn: "ID_Modelo");
                });

            migrationBuilder.CreateTable(
                name: "HistoricoAcoes",
                columns: table => new
                {
                    ID_Historico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Acao = table.Column<int>(type: "int", nullable: false),
                    ID_Utilizador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ID_Alvo = table.Column<int>(type: "int", nullable: true),
                    TipoAlvo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Razao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Historic__ECA887958CE82E6F", x => x.ID_Historico);
                    table.ForeignKey(
                        name: "FK__Historico__ID_Ac__7F2BE32F",
                        column: x => x.ID_Acao,
                        principalTable: "Acao",
                        principalColumn: "ID_Acao");
                });

            migrationBuilder.CreateTable(
                name: "Anuncio",
                columns: table => new
                {
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notificacao = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Ativo"),
                    Visualizacoes = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    ID_Veiculo = table.Column<int>(type: "int", nullable: false),
                    ID_Vendedor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ID_Localizacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Anuncio__D8875FB6A2E65C9A", x => x.ID_Anuncio);
                    table.ForeignKey(
                        name: "FK_Anuncio_AspNetUsers",
                        column: x => x.ID_Vendedor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Anuncio__ID_Loca__619B8048",
                        column: x => x.ID_Localizacao,
                        principalTable: "Localizacao",
                        principalColumn: "ID_Localizacao");
                    table.ForeignKey(
                        name: "FK__Anuncio__ID_Veic__5FB337D6",
                        column: x => x.ID_Veiculo,
                        principalTable: "Veiculo",
                        principalColumn: "ID_Veiculo");
                });

            migrationBuilder.CreateTable(
                name: "Imagem",
                columns: table => new
                {
                    ID_Imagem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Veiculo = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Imagem__5A31EE7D452C5C8C", x => x.ID_Imagem);
                    table.ForeignKey(
                        name: "FK__Imagem__ID_Veicu__6477ECF3",
                        column: x => x.ID_Veiculo,
                        principalTable: "Veiculo",
                        principalColumn: "ID_Veiculo");
                });

            migrationBuilder.CreateTable(
                name: "Compra",
                columns: table => new
                {
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false),
                    ID_Comprador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Concluida"),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Compra__D8875FB64E0009E5", x => x.ID_Anuncio);
                    table.ForeignKey(
                        name: "FK__Compra__ID_Anunc__75A278F5",
                        column: x => x.ID_Anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "ID_Anuncio");
                });

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

            migrationBuilder.CreateTable(
                name: "VisitaReserva",
                columns: table => new
                {
                    ID_Reserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataVisita = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pendente"),
                    ID_Anuncio = table.Column<int>(type: "int", nullable: false),
                    ID_Comprador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VisitaRe__12CAD9F4E4121AE5", x => x.ID_Reserva);
                    table.ForeignKey(
                        name: "FK_VisitaReserva_Anuncio_ID_Anuncio",
                        column: x => x.ID_Anuncio,
                        principalTable: "Anuncio",
                        principalColumn: "ID_Anuncio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitaReserva_AspNetUsers_ID_Comprador",
                        column: x => x.ID_Comprador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acao_ID_TipoAcao",
                table: "Acao",
                column: "ID_TipoAcao");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_ID_Localizacao",
                table: "Anuncio",
                column: "ID_Localizacao");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_ID_Veiculo",
                table: "Anuncio",
                column: "ID_Veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_Anuncio_ID_Vendedor",
                table: "Anuncio",
                column: "ID_Vendedor");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_ID_Anuncio",
                table: "Favorito",
                column: "ID_Anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_User_Anuncio",
                table: "Favorito",
                columns: new[] { "ID_Utilizador", "ID_Anuncio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_ID_Classe",
                table: "FiltrosFavoritos",
                column: "ID_Classe");

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_ID_Combustivel",
                table: "FiltrosFavoritos",
                column: "ID_Combustivel");

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_ID_Localizacao",
                table: "FiltrosFavoritos",
                column: "ID_Localizacao");

            migrationBuilder.CreateIndex(
                name: "IX_FiltrosFavoritos_ID_Marca",
                table: "FiltrosFavoritos",
                column: "ID_Marca");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoAcoes_ID_Acao",
                table: "HistoricoAcoes",
                column: "ID_Acao");

            migrationBuilder.CreateIndex(
                name: "IX_Imagem_ID_Veiculo",
                table: "Imagem",
                column: "ID_Veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_Modelo_ID_Marca",
                table: "Modelo",
                column: "ID_Marca");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId",
                table: "UserBlocks",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_ID_Classe",
                table: "Veiculo",
                column: "ID_Classe");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_ID_Combustivel",
                table: "Veiculo",
                column: "ID_Combustivel");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_ID_Modelo",
                table: "Veiculo",
                column: "ID_Modelo");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_ID_Vendedor",
                table: "Veiculo",
                column: "ID_Vendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_IdMarca",
                table: "Veiculo",
                column: "IdMarca");

            migrationBuilder.CreateIndex(
                name: "IX_Venda_IdAnuncio",
                table: "Venda",
                column: "IdAnuncio");

            migrationBuilder.CreateIndex(
                name: "IX_Venda_IdComprador",
                table: "Venda",
                column: "IdComprador");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaReserva_ID_Anuncio",
                table: "VisitaReserva",
                column: "ID_Anuncio");

            migrationBuilder.CreateIndex(
                name: "IX_VisitaReserva_ID_Comprador",
                table: "VisitaReserva",
                column: "ID_Comprador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Compra");

            migrationBuilder.DropTable(
                name: "Comprador");

            migrationBuilder.DropTable(
                name: "Favorito");

            migrationBuilder.DropTable(
                name: "FiltrosFavoritos");

            migrationBuilder.DropTable(
                name: "HistoricoAcoes");

            migrationBuilder.DropTable(
                name: "Imagem");

            migrationBuilder.DropTable(
                name: "SitePageView");

            migrationBuilder.DropTable(
                name: "UserBlocks");

            migrationBuilder.DropTable(
                name: "Venda");

            migrationBuilder.DropTable(
                name: "Vendedor");

            migrationBuilder.DropTable(
                name: "VisitaReserva");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Acao");

            migrationBuilder.DropTable(
                name: "Anuncio");

            migrationBuilder.DropTable(
                name: "TipoAcao");

            migrationBuilder.DropTable(
                name: "Localizacao");

            migrationBuilder.DropTable(
                name: "Veiculo");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Classe");

            migrationBuilder.DropTable(
                name: "Combustivel");

            migrationBuilder.DropTable(
                name: "Modelo");

            migrationBuilder.DropTable(
                name: "Marca");
        }
    }
}
