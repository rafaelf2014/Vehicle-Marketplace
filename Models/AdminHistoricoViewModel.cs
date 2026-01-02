namespace CliCarProject.Models
{
    public class AdminHistoricoViewModel
    {
        public string? SearchTerm { get; set; }
        public string? CurrentSort { get; set; }
        public string? CurrentOrder { get; set; }

        public List<HistoricoItem> Itens { get; set; } = new();

        public class HistoricoItem
        {
            public int IdHistorico { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Acao { get; set; } = string.Empty;
            public DateTime DataHora { get; set; }
        }
    }
}