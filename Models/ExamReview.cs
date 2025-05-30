using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamReview
    {
        [Key]
        public int ReviewID { get; set; }
        public int ExamQuestionID { get; set; }
        public int ReviewerID { get; set; }
        public string ReviewResult { get; set; }
        public string ReviewNotes { get; set; }

        public ExamQuestion ExamQuestion { get; set; }
        public User Reviewer { get; set; }
    }
}