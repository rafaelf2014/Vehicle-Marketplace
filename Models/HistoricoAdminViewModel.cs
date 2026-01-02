namespace CliCarProject.Models
{
    public class HistoricoAdminViewModel
    {
        public int IdHistorico { get; set; }

        public string IdUtilizador { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string NomeAcao { get; set; } = null!;

        public string TipoAcao { get; set; } = null!;

        public int? IdAlvo { get; set; }

        public string? TipoAlvo { get; set; }

        public string? Razao { get; set; }

        public DateTime? DataHora { get; set; }
    }
}

