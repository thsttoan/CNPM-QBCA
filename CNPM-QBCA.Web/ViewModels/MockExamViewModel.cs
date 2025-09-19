using QBCA.Models;
using System.Collections.Generic;

namespace CNPM_QBCA.Models
{
    public class MockExamViewModel
    {
        public int AssignmentID { get; set; }

        public string ExamTitle { get; set; } = string.Empty;

        public List<Question> Questions { get; set; } = new();

        public Dictionary<int, string> Answers { get; set; } = new();

        public string Feedback { get; set; } = string.Empty;
    }
}
