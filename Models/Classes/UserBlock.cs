using System;
using System.ComponentModel.DataAnnotations;

namespace CliCarProject.Models.Classes
{
    public class UserBlock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
    }
}