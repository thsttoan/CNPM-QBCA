using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamReview
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ExamQuestionID { get; set; }

        [Required]
        public int ReviewerID { get; set; }

        [Required]
        [StringLength(100)]
        public string ReviewResult { get; set; }

        [StringLength(500)]
        public string ReviewNotes { get; set; }

        public ExamQuestion ExamQuestion { get; set; }
        public User Reviewer { get; set; }

        // (optional) Thêm thời gian review nếu muốn đồng bộ với các chỗ khác
        public DateTime? ReviewedAt { get; set; }
    }
}