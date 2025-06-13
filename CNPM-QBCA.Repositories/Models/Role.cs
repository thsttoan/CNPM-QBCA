using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Make sure this is included

namespace QBCA.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RoleID")]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        // Add this navigation property for the relationship
        public virtual ICollection<User> Users { get; set; } // This will represent all users belonging to this role

        // It's good practice to initialize collection properties in the constructor
        public Role()
        {
            Users = new HashSet<User>();
        }
    }
}