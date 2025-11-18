using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class TipoAcao
{
    public int IdTipoAcao { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Acao> Acaos { get; set; } = new List<Acao>();
}
