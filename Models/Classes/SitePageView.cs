using System;

namespace CliCarProject.Models.Classes
{
    public partial class SitePageView
    {
        public int IdSitePageView { get; set; }
        public DateTime VisitTime { get; set; } = DateTime.UtcNow;
        public string? Path { get; set; }
    }
}