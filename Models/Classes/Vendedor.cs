using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Models.Classes;

public partial class Vendedor
{
    public string IdUtilizador { get; set; } = null!;

    public string? Contacto { get; set; }

    public string? CodigoPostal { get; set; }

    public string? Nif { get; set; }

    public string? Tipo { get; set; }

    public virtual IdentityUser IdUtilizadorNavigation { get; set; } = null!;
}
