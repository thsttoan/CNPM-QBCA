using Microsoft.AspNetCore.Mvc;

namespace CNPM_QBCA.Models
{
    public class MockExamAnswer
    {
        public int MockExamAnswerID { get; set; }
        public int MockExamId { get; set; }
        public string LecturerId { get; set; }
        public string AnswerJson { get; set; }
        public DateTime SubmittedAt { get; set; }
        public virtual MockExam MockExam { get; set; }

    }
}
