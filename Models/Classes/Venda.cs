using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Identity;

public class Venda
{
    public int IdVenda { get; set; }
    public int IdAnuncio { get; set; }
    public string IdComprador { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal PrecoFinal { get; set; }

    
    public virtual Anuncio Anuncio { get; set; }
    public virtual IdentityUser Comprador { get; set; }
}