using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamQuestion
    {
        [Key]
        public int ExamQuestionID { get; set; }
        public int PlanID { get; set; }
        public int QuestionID { get; set; }
        public bool Approved { get; set; }
        public string Comment { get; set; }

        public ExamPlan ExamPlan { get; set; }
        public Question Question { get; set; }
        public ICollection<ExamReview> ExamReviews { get; set; }
    }
}