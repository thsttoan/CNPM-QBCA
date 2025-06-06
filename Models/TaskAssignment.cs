using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class TaskAssignment
    {
        [Key]
        public int AssignmentID { get; set; }
        public int AssignedBy { get; set; }

        [Required]
        public int AssignedTo { get; set; }

        [Required]
        public string TaskType { get; set; }

        [Required]
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Assigner { get; set; }
        public User Assignee { get; set; }
    }
}
