using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamQuestion
    {
        [Key]
        public int ExamQuestionID { get; set; }
        [Required]
        public int ExamID { get; set; }  

        [Required]
        public int ExamPlanID { get; set; }

        [Required]
        public int QuestionID { get; set; }

        public bool Approved { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime SubmittedAt { get; set; } 

        public ExamPlan ExamPlan { get; set; }
        public Question Question { get; set; }
        public ICollection<ExamReview> ExamReviews { get; set; } = new List<ExamReview>();
        public Exam Exam { get; set; }  
    }
}
