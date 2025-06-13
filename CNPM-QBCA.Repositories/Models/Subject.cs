using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }

        [Required]
        public string SubjectName { get; set; }

        public int CreatedBy { get; set; }
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Status { get; set; }

        public ICollection<CLO> CLOs { get; set; } = new List<CLO>();
        public ICollection<ExamPlan> ExamPlans { get; set; } = new List<ExamPlan>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<DifficultyLevel> DifficultyLevels { get; set; } = new List<DifficultyLevel>();

        // Đã xoá thuộc tính Plans vì Plan đã bị loại bỏ hoàn toàn!
    }
}