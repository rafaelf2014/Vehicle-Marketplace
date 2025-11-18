using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class FiltrosFavorito
{
    public int IdFiltroFavorito { get; set; }

    public string IdComprador { get; set; } = null!;

    public int? IdCombustivel { get; set; }

    public int? IdClasse { get; set; }

    public int? IdLocalizacao { get; set; }

    public int? IdMarca { get; set; }

    public virtual Classe? IdClasseNavigation { get; set; }

    public virtual Combustivel? IdCombustivelNavigation { get; set; }

    public virtual Localizacao? IdLocalizacaoNavigation { get; set; }

    public virtual Marca? IdMarcaNavigation { get; set; }
}
