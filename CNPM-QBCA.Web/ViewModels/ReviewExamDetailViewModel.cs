namespace QBCA.ViewModels.ReviewExam
{
    public class ReviewExamDetailViewModel
    {
        public int ReviewID { get; set; }
        public string ExamTitle { get; set; }
        public string LecturerName { get; set; }
        public string SubjectName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime ReviewedAt { get; set; }
    }
}
