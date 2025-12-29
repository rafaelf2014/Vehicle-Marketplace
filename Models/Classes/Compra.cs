using System;
using System.Collections.Generic;
using CliCarProject.Models.Classes;

namespace CliCarProject.Models.Classes;

public partial class Compra
{
    public int IdAnuncio { get; set; }

    public string IdComprador { get; set; } = null!;

    public DateTime? DataCompra { get; set; }

    public string? Estado { get; set; }

    public decimal Preco { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;
}
