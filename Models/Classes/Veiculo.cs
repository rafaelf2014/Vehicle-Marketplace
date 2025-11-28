using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Models.Classes;

public partial class Veiculo
{
    public int IdVeiculo { get; set; }

    public string? IdVendedor { get; set; } = null!;

    public int Ano { get; set; }

    public int? Quilometragem { get; set; }
    [Required]
    [RegularExpression("N|S|U", ErrorMessage = "Condição inválida.")]
    public string? Condicao { get; set; }

    [Required]
    [RegularExpression("M|A", ErrorMessage = "Condição inválida.")]
    public string? Caixa { get; set; }
    public int IdModelo { get; set; }

    public int IdMarca { get; set; }

    public int IdCombustivel { get; set; }

    public int IdClasse { get; set; }

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual Classe? IdClasseNavigation { get; set; }

    public virtual Combustivel? IdCombustivelNavigation { get; set; }

    public virtual Modelo? IdModeloNavigation { get; set; }
    public virtual Marca? IdMarcaNavigation { get; set; }
    public virtual IdentityUser? IdVendedorNavigation { get; set; } 

    public virtual ICollection<Imagem> Imagems { get; set; } = new List<Imagem>();
}
