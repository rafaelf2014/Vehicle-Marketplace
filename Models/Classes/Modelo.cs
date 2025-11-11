using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Modelo
{
    public int IdModelo { get; set; }

    public string Nome { get; set; } = null!;

    public int IdMarca { get; set; }

    public virtual Marca IdMarcaNavigation { get; set; } = null!;

    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
