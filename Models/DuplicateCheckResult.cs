using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class DuplicateCheckResult
    {
        [Key]
        public int CheckID { get; set; }
        public int QuestionID { get; set; }
        public int SimilarQuestionID { get; set; }
        public double SimilarityScore { get; set; }
        public string CheckType { get; set; }
        public DateTime CheckedAt { get; set; }

        public Question Question { get; set; }
        public Question SimilarQuestion { get; set; }
    }
}