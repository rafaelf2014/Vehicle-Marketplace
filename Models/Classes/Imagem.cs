using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Imagem
{
    public int IdImagem { get; set; }

    public int IdVeiculo { get; set; }

    public string Nome { get; set; }

    public virtual Veiculo? IdVeiculoNavigation { get; set; }
}
