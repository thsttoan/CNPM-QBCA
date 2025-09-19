using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Exam
    {
        [Key]
        public int ExamID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int DistributionID { get; set; }
        public ExamPlanDistribution ExamPlanDistribution { get; set; }
        public DateTime SubmitDate { get; set; } // Ngày nộp đề
        public int SubmittedBy { get; set; }     // Ai là người nộp
        public User Submitter { get; set; }

        public string Status { get; set; }       // Submitted, Approved, Rejected,...

        [Required]
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<ExamApproveTask> ApproveTasks { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public int ExamPlanID { get; set; }
    }
}
