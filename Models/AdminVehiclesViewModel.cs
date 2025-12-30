using System;
using System.Collections.Generic;

namespace CliCarProject.Models
{
    public class AdminVehiclesViewModel
    {
        public int TotalVeiculos { get; set; }

        public List<VehicleItem> Vehicles { get; set; } = new();

        public class VehicleItem
        {
            public int IdVeiculo { get; set; }
            public string Marca { get; set; } = string.Empty;
            public string Modelo { get; set; } = string.Empty;
            public int? Ano { get; set; }
            public string Proprietario { get; set; } = string.Empty;
            public bool Disponivel { get; set; }   // novo
        }
    }
}