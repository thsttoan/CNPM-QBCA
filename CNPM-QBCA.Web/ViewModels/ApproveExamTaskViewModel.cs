using Microsoft.AspNetCore.Mvc;

namespace CNPM_QBCA.ViewModels
{
    public class ApproveExamTaskViewModel
    {


        public int TaskID { get; set; }
        public string ExamTitle { get; set; }
        public string Status { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
