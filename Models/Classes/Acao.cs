using CliCarProject.Models.Classes;
using System;
using System.Collections.Generic;

namespace CliCarProject.Models;

public partial class Acao
{
    public int IdAcao { get; set; }

    public int IdTipoAcao { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public string? TipoAlvo { get; set; }

    public virtual ICollection<HistoricoAco> HistoricoAcos { get; set; } = new List<HistoricoAco>();

    public virtual TipoAcao IdTipoAcaoNavigation { get; set; } = null!;
}
