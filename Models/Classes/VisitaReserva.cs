using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class VisitaReserva
{
    public int IdReserva { get; set; }

    public DateTime? DataVisita { get; set; }

    public DateTime? DataExpiracao { get; set; }

    public string? Estado { get; set; }

    public int IdAnuncio { get; set; }

    public string IdComprador { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;
}
