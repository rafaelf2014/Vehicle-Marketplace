using System;
using System.Collections.Generic;

namespace CliCarProject.Models
{
    public class UsersViewModel
    {
        public int TotalAccountsCreated { get; set; }
        public int TotalCompradores { get; set; }
        public int TotalVendedores { get; set; }

        public List<UserListItem> Users { get; set; } = new();

        public class UserListItem
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public DateTime? CreatedAt { get; set; }
            public bool IsBlocked { get; set; }
            public bool IsSuperAdmin { get; set; } // NOVO
        }
    }
}