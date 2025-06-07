using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamPlan
    {
        [Key]
        public int PlanID { get; set; }

        [Required]
        public int SubjectID { get; set; }
        public Subject Subject { get; set; }

        [Required(ErrorMessage = "Plan Name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Thêm property này nếu bạn có thông tin ai tạo kế hoạch
        public int? CreatedBy { get; set; }
        public User Creator { get; set; }

        public ICollection<ExamPlanDistribution> Distributions { get; set; } = new List<ExamPlanDistribution>();
        public ICollection<SubmissionTable> SubmissionTables { get; set; } = new List<SubmissionTable>();
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}