using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class SubmissionTable
    {
        [Key]
        public int SubmissionID { get; set; }

        [Required]
        public int PlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        public User Creator { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string FinalStatus { get; set; }

        [StringLength(512)]
        public string ReviewerComment { get; set; }

        public int? ApprovedBy { get; set; }
        public User Approver { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public double? DuplicateRate { get; set; }
        public string DuplicateReport { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}