using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }
        public int SubjectID { get; set; }
        public int CLOID { get; set; }
        public int DifficultyLevelID { get; set; }
        public string Content { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        public Subject Subject { get; set; }
        public CLO CLO { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public User Creator { get; set; }
        public ICollection<QuestionUpload> QuestionUploads { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
        public ICollection<DuplicateCheckResult> DuplicateCheckResults { get; set; }
        public ICollection<DuplicateCheckResult> SimilarQuestions { get; set; }
    }
}