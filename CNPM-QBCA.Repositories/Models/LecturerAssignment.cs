using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class LecturerAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        public int ExamPlanID { get; set; }
        public int DistributionID { get; set; }

        public int AssignedBy { get; set; }
        public int AssignedTo { get; set; }

        public string TaskType { get; set; }
        public string Status { get; set; }

        public DateTime AssignedAt { get; set; }
        public DateTime Dealine { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
  
        public string LecturerName { get; set; }
        public string ExamTitle { get; set; }
        public bool IsReviewed { get; set; }
    }
}
