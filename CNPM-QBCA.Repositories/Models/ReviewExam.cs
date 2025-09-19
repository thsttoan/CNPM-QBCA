using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ReviewExam
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ExamPlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        [Required]
        public int? DistributionID { get; set; }
        public ExamPlanDistribution Distribution { get; set; }

        [Required]
        public int ReviewerID { get; set; }
        public User Reviewer { get; set; }

        public string Comment { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime ReviewedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
