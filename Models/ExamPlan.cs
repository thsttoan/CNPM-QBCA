using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamPlan
    {
        [Key]
        public int PlanID { get; set; }
        public int SubjectID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NumOfQuestions { get; set; }
        public string Status { get; set; }

        public Subject Subject { get; set; }
        public User Creator { get; set; }
        public ICollection<ExamQuestion> ExamQuestions { get; set; }
        public ICollection<SubmissionTable> SubmissionTables { get; set; }
    }
}