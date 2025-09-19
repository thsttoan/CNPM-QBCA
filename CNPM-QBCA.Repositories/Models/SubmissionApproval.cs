using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBCA.Models
{
    public class SubmissionApproval
    {
        [Key]
        public int SubmissionApprovalID { get; set; }

        [Required]
        public int SubmissionTableID { get; set; }

        public virtual SubmissionTable? SubmissionTable { get; set; }

        [Required]
        public int AssignedPlanID { get; set; }

        public virtual AssignedPlan? AssignedPlan { get; set; }

        [Required]
        public int ApprovedBy { get; set; }

        public virtual User? Approver { get; set; }

        public DateTime ApprovedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Feedback { get; set; }
    }
}
