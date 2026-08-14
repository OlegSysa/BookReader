using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BookReader.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string? ExternalId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
