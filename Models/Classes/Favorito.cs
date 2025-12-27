using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CliCarProject.Models.Classes
{
    public class Favorito
    {
        [Key]
        public int IdFavorito { get; set; }

        [Required]
        public string IdUtilizador { get; set; }

        [Required]
        public int IdAnuncio { get; set; }

        // Propriedades de Navegação
        [ForeignKey("IdUtilizador")]
        public virtual IdentityUser Utilizador { get; set; }

        [ForeignKey("IdAnuncio")]
        public virtual Anuncio Anuncio { get; set; }
    }
}