using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [Required]
        public int CLOID { get; set; }

        [Required]
        public int DifficultyLevelID { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Status { get; set; }

        // >>>>> BỔ SUNG CHO SUBMISSION TABLE <<<<<
        public int? SubmissionID { get; set; }
        public SubmissionTable? SubmissionTable { get; set; }

        public Subject? Subject { get; set; }
        public CLO? CLO { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        public User? Creator { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
        public ICollection<DuplicateCheckResult> DuplicateCheckResults { get; set; } = new List<DuplicateCheckResult>();
        public ICollection<DuplicateCheckResult> SimilarQuestions { get; set; } = new List<DuplicateCheckResult>();
    }
}