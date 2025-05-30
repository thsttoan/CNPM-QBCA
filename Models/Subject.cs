using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class Subject
    {
        [Key]
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }

        public User Creator { get; set; }
        public ICollection<CLO> CLOs { get; set; }
        public ICollection<ExamPlan> ExamPlans { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<DifficultyLevel> DifficultyLevels { get; set; }
    }
}