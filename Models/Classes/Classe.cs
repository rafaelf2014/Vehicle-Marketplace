using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Classe
{
    public int IdClasse { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<FiltrosFavorito> FiltrosFavoritos { get; set; } = new List<FiltrosFavorito>();

    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
