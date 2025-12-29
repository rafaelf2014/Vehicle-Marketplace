using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Models.Classes;

public partial class Administrador
{
    public string IdUtilizador { get; set; } = null!;

    public virtual IdentityUser IdUtilizadorNavigation { get; set; } = null!;
}
