using System;
using System.Collections.Generic;

namespace CliCarProject.Models.Classes;

public partial class Localizacao
{
    public int IdLocalizacao { get; set; }

    public string Distrito { get; set; } = null!;

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();

    public virtual ICollection<FiltrosFavorito> FiltrosFavoritos { get; set; } = new List<FiltrosFavorito>();
}
