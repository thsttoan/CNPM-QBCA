using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Login
    {
        [Key]
        public int LoginID { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleID { get; set; }
        public DateTime LastLogin { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
    }
}