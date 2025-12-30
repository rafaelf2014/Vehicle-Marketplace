using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Anuncio
{
    public int IdAnuncio { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }
    public bool Notificacao { get; set; }

    public string? Estado { get; set; }
    public int Visualizacoes { get; set; }

    public DateTime? DataCriacao { get; set; }

    public DateTime? DataAtualizacao { get; set; }

    public int IdVeiculo { get; set; }

    public string? IdVendedor { get; set; } 

    public int IdLocalizacao { get; set; }

    public virtual Compra? Compra { get; set; }

    public virtual Localizacao? IdLocalizacaoNavigation { get; set; } 

    public virtual Veiculo? IdVeiculoNavigation { get; set; } 

    public virtual IdentityUser? IdVendedorNavigation { get; set; }

    public virtual ICollection<VisitaReserva> VisitaReservas { get; set; } = new List<VisitaReserva>();
}
