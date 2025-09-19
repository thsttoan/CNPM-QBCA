using QBCA.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_QBCA.Models

{
    public class MockExam
    {
        [Key]
        public int MockExamID { get; set; }

        [ForeignKey("TaskAssignment")]
        public int AssignmentID { get; set; }

        public int LecturerID { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string AnswersJson { get; set; } = string.Empty;

        public string Feedback { get; set; } = string.Empty;
        public virtual ICollection<MockExamAnswer> Answers { get; set; } = new List<MockExamAnswer>();
        public virtual ICollection<MockFeedback> Feedbacks { get; set; } = new List<MockFeedback>();
        public ICollection<MockQuestion> Questions { get; set; } = new List<MockQuestion>();
        public virtual ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();

        // Navigation (nếu cần dùng)
        public TaskAssignment? TaskAssignment { get; set; }
    }
}
