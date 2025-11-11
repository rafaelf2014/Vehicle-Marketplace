using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class HistoricoAco
{
    public int IdHistorico { get; set; }

    public int IdAcao { get; set; }

    public string IdUtilizador { get; set; } = null!;

    public int? IdAlvo { get; set; }

    public string? TipoAlvo { get; set; }

    public string? Razao { get; set; }

    public DateTime? DataHora { get; set; }

    public virtual Acao IdAcaoNavigation { get; set; } = null!;
}
