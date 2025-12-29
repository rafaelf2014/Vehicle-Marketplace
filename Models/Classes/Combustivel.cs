using CliCarProject.Models.Classes;
using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Combustivel
{
    public int IdCombustivel { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<FiltrosFavorito> FiltrosFavoritos { get; set; } = new List<FiltrosFavorito>();

    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
