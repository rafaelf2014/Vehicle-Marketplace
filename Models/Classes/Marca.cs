using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<FiltrosFavorito> FiltrosFavoritos { get; set; } = new List<FiltrosFavorito>();

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
}
