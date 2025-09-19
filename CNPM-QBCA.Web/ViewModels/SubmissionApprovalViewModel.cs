using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QBCA.Models;

namespace QBCA.ViewModels
{
    public class SubmissionApprovalViewModel
    {
        public int SubmissionApprovalID { get; set; }

        [Required]
        public int SubmissionTableID { get; set; }

        [Required]
        public int ApprovedByID { get; set; }

        [Display(Name = "Submission Title")]
        public string SubmissionTitle { get; set; }

        [Display(Name = "Subject")]
        public string SubjectName { get; set; }

        public string Status { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedByName { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        public string Feedback { get; set; }


        public List<string> AllSubjects { get; set; } = new();
        public List<string> AllStatuses { get; set; } = new();

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }


        public List<SubmissionTable> AllSubmissions { get; set; } = new();
        public List<User> AllApprovers { get; set; } = new();
    }
}
