using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CliCarProject.Models.Classes;
using CliCarProject.Models;
using Microsoft.AspNetCore.Identity;


namespace CliCarProject.Data;

public partial class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acao> Acaos { get; set; }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    
    public virtual DbSet<Classe> Classes { get; set; }

    public virtual DbSet<Combustivel> Combustivels { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<FiltrosFavorito> FiltrosFavoritos { get; set; }

    public virtual DbSet<HistoricoAco> HistoricoAcoes { get; set; }

    public virtual DbSet<Imagem> Imagems { get; set; }

    public virtual DbSet<Localizacao> Localizacaos { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<TipoAcao> TipoAcaos { get; set; }

    public virtual DbSet<Veiculo> Veiculos { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    public virtual DbSet<VisitaReserva> VisitaReservas { get; set; }
    public DbSet<Favorito> Favoritos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(e => e.IdVenda);
            entity.ToTable("Venda");

            entity.HasOne(d => d.Anuncio)
                .WithMany()
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.NoAction); 

            entity.HasOne(d => d.Comprador)
                .WithMany()
                .HasForeignKey(d => d.IdComprador);
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            // Define a Chave Primária
            entity.HasKey(e => e.IdFavorito);

            // Define o nome da tabela
            entity.ToTable("Favorito");

            // Configura as colunas
            entity.Property(e => e.IdFavorito).HasColumnName("ID_Favorito");
            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");

            // Cria um índice único para evitar duplicados (Mesmo User + Mesmo Anúncio)
            entity.HasIndex(e => new { e.IdUtilizador, e.IdAnuncio }, "IX_Favorito_User_Anuncio")
                .IsUnique();

            // Configura a relação com AspNetUsers (IdentityUser)
            entity.HasOne(d => d.Utilizador)
                .WithMany() // Um utilizador pode ter muitos favoritos
                .HasForeignKey(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.Cascade) // Se o user for apagado, apaga os favoritos
                .HasConstraintName("FK_Favorito_AspNetUsers");

            // Configura a relação com a tabela Anuncio
            entity.HasOne(d => d.Anuncio)
                .WithMany() // Um anúncio pode estar nos favoritos de muitos users
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.Cascade) // Se o anúncio for apagado, remove-o dos favoritos
                .HasConstraintName("FK_Favorito_Anuncio");
        });

        modelBuilder.Entity<Acao>(entity =>
        {
            entity.HasKey(e => e.IdAcao).HasName("PK__Acao__4EE45891E21367C6");

            entity.ToTable("Acao");

            entity.Property(e => e.IdAcao).HasColumnName("ID_Acao");
            entity.Property(e => e.Descricao).HasMaxLength(255);
            entity.Property(e => e.IdTipoAcao).HasColumnName("ID_TipoAcao");
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.TipoAlvo).HasMaxLength(50);

            entity.HasOne(d => d.IdTipoAcaoNavigation).WithMany(p => p.Acaos)
                .HasForeignKey(d => d.IdTipoAcao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Acao__ID_TipoAca__7B5B524B");
        });

        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.IdUtilizador);

            entity.ToTable("Administrador");

            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithOne()
                .HasForeignKey<Administrador>(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Administrador_AspNetUsers");
        });

        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.IdAnuncio).HasName("PK__Anuncio__D8875FB6A2E65C9A");

            entity.ToTable("Anuncio");

            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.DataAtualizacao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descricao).HasMaxLength(255);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Ativo");
            entity.Property(e => e.IdLocalizacao).HasColumnName("ID_Localizacao");
            entity.Property(e => e.IdVeiculo).HasColumnName("ID_Veiculo");
            entity.Property(e => e.IdVendedor)
                .HasMaxLength(450)
                .HasColumnName("ID_Vendedor");
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Titulo).HasMaxLength(100);

            entity.HasOne(d => d.IdLocalizacaoNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdLocalizacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Loca__619B8048");

            entity.HasOne(d => d.IdVeiculoNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdVeiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Veic__5FB337D6");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany()
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Anuncio_AspNetUsers");
        });

        modelBuilder.Entity<Classe>(entity =>
        {
            entity.HasKey(e => e.IdClasse).HasName("PK__Classe__7C4BBB54FAA85776");

            entity.ToTable("Classe");

            entity.Property(e => e.IdClasse).HasColumnName("ID_Classe");
            entity.Property(e => e.Nome).HasMaxLength(50);
        });

        modelBuilder.Entity<Combustivel>(entity =>
        {
            entity.HasKey(e => e.IdCombustivel).HasName("PK__Combusti__F8AAF41E4F0B740D");

            entity.ToTable("Combustivel");

            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.Tipo).HasMaxLength(50);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdAnuncio).HasName("PK__Compra__D8875FB64E0009E5");

            entity.ToTable("Compra");

            entity.Property(e => e.IdAnuncio)
                .ValueGeneratedNever()
                .HasColumnName("ID_Anuncio");
            entity.Property(e => e.DataCompra).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Concluida");
            entity.Property(e => e.IdComprador)
                .HasMaxLength(450)
                .HasColumnName("ID_Comprador");
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdAnuncioNavigation).WithOne(p => p.Compra)
                .HasForeignKey<Compra>(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ID_Anunc__75A278F5");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.HasKey(e => e.IdUtilizador);

            entity.ToTable("Comprador");

            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Contacto)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Morada).HasMaxLength(100);

            entity.HasOne(d => d.IdUtilizadorNavigation).WithOne()
                .HasForeignKey<Comprador>(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comprador_AspNetUsers");
        });

        modelBuilder.Entity<FiltrosFavorito>(entity =>
        {
            entity.HasKey(e => e.IdFiltroFavorito).HasName("PK__FiltrosF__410905C7C5E79498");

            entity.Property(e => e.IdFiltroFavorito).HasColumnName("ID_FiltroFavorito");
            entity.Property(e => e.IdClasse).HasColumnName("ID_Classe");
            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.IdComprador)
                .HasMaxLength(450)
                .HasColumnName("ID_Comprador");
            entity.Property(e => e.IdLocalizacao).HasColumnName("ID_Localizacao");
            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");

            entity.HasOne(d => d.IdClasseNavigation).WithMany(p => p.FiltrosFavoritos)
                .HasForeignKey(d => d.IdClasse)
                .HasConstraintName("FK__FiltrosFa__ID_Cl__693CA210");

            entity.HasOne(d => d.IdCombustivelNavigation).WithMany(p => p.FiltrosFavoritos)
                .HasForeignKey(d => d.IdCombustivel)
                .HasConstraintName("FK__FiltrosFa__ID_Co__68487DD7");

            entity.HasOne(d => d.IdLocalizacaoNavigation).WithMany(p => p.FiltrosFavoritos)
                .HasForeignKey(d => d.IdLocalizacao)
                .HasConstraintName("FK__FiltrosFa__ID_Lo__6A30C649");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.FiltrosFavoritos)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("FK__FiltrosFa__ID_Ma__6B24EA82");
        });

        modelBuilder.Entity<HistoricoAco>(entity =>
        {
            entity.HasKey(e => e.IdHistorico).HasName("PK__Historic__ECA887958CE82E6F");

            entity.Property(e => e.IdHistorico).HasColumnName("ID_Historico");
            entity.Property(e => e.DataHora).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IdAcao).HasColumnName("ID_Acao");
            entity.Property(e => e.IdAlvo).HasColumnName("ID_Alvo");
            entity.Property(e => e.IdUtilizador)
                .HasMaxLength(450)
                .HasColumnName("ID_Utilizador");
            entity.Property(e => e.Razao).HasMaxLength(255);
            entity.Property(e => e.TipoAlvo).HasMaxLength(50);

            entity.HasOne(d => d.IdAcaoNavigation).WithMany(p => p.HistoricoAcos)
                .HasForeignKey(d => d.IdAcao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__ID_Ac__7F2BE32F");
        });

        modelBuilder.Entity<Imagem>(entity =>
        {
            entity.HasKey(e => e.IdImagem).HasName("PK__Imagem__5A31EE7D452C5C8C");

            entity.ToTable("Imagem");

            entity.Property(e => e.IdImagem).HasColumnName("ID_Imagem");
            entity.Property(e => e.IdVeiculo).HasColumnName("ID_Veiculo");
            entity.Property(e => e.Nome).HasMaxLength(50);

            entity.HasOne(d => d.IdVeiculoNavigation).WithMany(p => p.Imagems)
                .HasForeignKey(d => d.IdVeiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Imagem__ID_Veicu__6477ECF3");
        });

        modelBuilder.Entity<Localizacao>(entity =>
        {
            entity.HasKey(e => e.IdLocalizacao).HasName("PK__Localiza__72A2EF98171EE69F");

            entity.ToTable("Localizacao");

            entity.Property(e => e.IdLocalizacao).HasColumnName("ID_Localizacao");
            entity.Property(e => e.Distrito).HasMaxLength(100);
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__Marca__9B8F8DB2441923EF");

            entity.ToTable("Marca");

            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.Nome).HasMaxLength(50);
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.IdModelo).HasName("PK__Modelo__813C2372542DB0DE");

            entity.ToTable("Modelo");

            entity.Property(e => e.IdModelo).HasColumnName("ID_Modelo");
            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.Nome).HasMaxLength(50);

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modelo__ID_Marca__5165187F");
        });

        modelBuilder.Entity<TipoAcao>(entity =>
        {
            entity.HasKey(e => e.IdTipoAcao).HasName("PK__TipoAcao__D8411B34A8D3749C");

            entity.ToTable("TipoAcao");

            entity.Property(e => e.IdTipoAcao).HasColumnName("ID_TipoAcao");
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(e => e.IdVeiculo).HasName("PK__Veiculo__808FFECF1FB610CC");

            entity.ToTable("Veiculo");

            entity.Property(e => e.IdVeiculo).HasColumnName("ID_Veiculo");
            entity.Property(e => e.Condicao)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.IdClasse).HasColumnName("ID_Classe");
            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.IdModelo).HasColumnName("ID_Modelo");
            entity.Property(e => e.IdVendedor)
                .HasMaxLength(450)
                .HasColumnName("ID_Vendedor");

            entity.HasOne(d => d.IdClasseNavigation).WithMany(p => p.Veiculos)
                .HasForeignKey(d => d.IdClasse)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Veiculo__ID_Clas__59FA5E80");

            entity.HasOne(d => d.IdCombustivelNavigation).WithMany(p => p.Veiculos)
                .HasForeignKey(d => d.IdCombustivel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Veiculo__ID_Comb__59063A47");

            entity.HasOne(d => d.IdModeloNavigation).WithMany(p => p.Veiculos)
                .HasForeignKey(d => d.IdModelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Veiculo__ID_Mode__5812160E");

            entity.HasOne(d => d.IdMarcaNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Veiculo_Marca");

            entity.HasOne(d => d.IdVendedorNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Veiculo_AspNetUsers");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasKey(e => e.IdUtilizador);

            entity.ToTable("Vendedor");

            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Contacto)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nif)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("NIF");
            entity.Property(e => e.Tipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.IdUtilizadorNavigation).WithOne()
                .HasForeignKey<Vendedor>(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vendedor_AspNetUsers");
        });

        modelBuilder.Entity<VisitaReserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__VisitaRe__12CAD9F4E4121AE5");

            entity.ToTable("VisitaReserva");

            entity.Property(e => e.IdReserva).HasColumnName("ID_Reserva");
            entity.Property(e => e.DataVisita).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendente");
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.IdComprador)
                .HasMaxLength(450)
                .HasColumnName("ID_Comprador");

            entity.HasOne(d => d.IdAnuncioNavigation)
                .WithMany(p => p.VisitaReservas)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Comprador)
                .WithMany()
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
