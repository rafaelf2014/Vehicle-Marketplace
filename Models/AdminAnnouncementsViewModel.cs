using System;
using System.Collections.Generic;

namespace CliCarProject.Models
{
    public class AdminAnnouncementsViewModel
    {
        public int TotalAnuncios { get; set; }
        public string? MarcaMaisPresente { get; set; }
        public List<AnnouncementItem> Announcements { get; set; } = new();

        public class AnnouncementItem
        {
            public int IdAnuncio { get; set; }
            public string Criador { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public DateTime? DataCriacao { get; set; }
            public string Marca { get; set; } = string.Empty;
        }
    }
}