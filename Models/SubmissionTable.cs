using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class SubmissionTable
    {
        [Key]
        public int SubmissionID { get; set; }
        public int PlanID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FinalStatus { get; set; }

        public ExamPlan ExamPlan { get; set; }
        public User Creator { get; set; }
    }
}