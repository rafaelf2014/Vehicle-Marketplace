using System;
using System.Collections.Generic;

namespace CliCarProject.Models
{
    public class AdminAnnouncementsViewModel
    {
        public int TotalAnuncios { get; set; }

        public List<AnnouncementItem> Announcements { get; set; } = new();

        public class AnnouncementItem
        {
            public int IdAnuncio { get; set; }
            public string Criador { get; set; } = string.Empty;
            public string? Estado { get; set; }
            public DateTime? DataCriacao { get; set; }
        }
    }
}