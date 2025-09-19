using Microsoft.AspNetCore.Mvc;
using QBCA.Models;

namespace CNPM_QBCA.ViewModels
{
    public class ApproveExamDetailViewModel
    {
            public ExamApproveTask Task { get; set; }
            public SubmissionTable Submission { get; set; }
        
    }
}
