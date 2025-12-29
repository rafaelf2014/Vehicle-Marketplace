using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Models.Classes;

public partial class Comprador
{
    public string IdUtilizador { get; set; } = null!;

    public string Morada { get; set; } = null!;

    public string? Contacto { get; set; }

    public string? CodigoPostal { get; set; }

    public virtual IdentityUser IdUtilizadorNavigation { get; set; } = null!;
}
