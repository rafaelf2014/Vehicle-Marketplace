namespace CliCarProject.Models
{
    using System.Collections.Generic;

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalAnuncios { get; set; }
        public int TotalVeiculos { get; set; }

        // Novas propriedades para o dashboard
        public int TotalSiteVisits { get; set; }
        public int TotalAccountsCreated { get; set; }

        // Visitas na última semana (rolling 7 dias)
        public int WeeklySiteVisits { get; set; }

        // Dados para o gráfico de visitas ao longo do tempo
        public List<string> VisitLabels { get; set; } = new();
        public List<int> VisitCounts { get; set; } = new();

        // Lista de viaturas para sidebar (id, marca, modelo, ano, utilizador)
        public List<VehicleListItem> Vehicles { get; set; } = new();

        public class VehicleListItem
        {
            public int IdVeiculo { get; set; }
            public string Marca { get; set; } = string.Empty;
            public string Modelo { get; set; } = string.Empty;
            public int? Ano { get; set; }
            public string Proprietario { get; set; } = string.Empty;
        }
    }
}