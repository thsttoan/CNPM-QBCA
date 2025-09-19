using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public enum AssignedPlanStatus
    {
        [Display(Name = "Assigned")]
        Assigned,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Overdue")]
        Overdue,

        [Display(Name = "Final Approved")]
        FinalApproved,

        [Display(Name = "Rejected (Final)")]
        RejectedFinal
    }

    public enum TaskTypeEnum
    {
        [Display(Name = "Review Exam")]
        ReviewExam,

        [Display(Name = "Check Difficulty")]
        CheckDifficulty,

        [Display(Name = "Write Feedback")]
        WriteFeedback,
        QuestionWriting
    }

    public class AssignedPlan
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ExamPlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        [Required]
        public int DistributionID { get; set; }
        public ExamPlanDistribution Distribution { get; set; }

        [Required]
        public int AssignedToID { get; set; }
        public User AssignedTo { get; set; }

        [Required]
        public int AssignedByID { get; set; }
        public User AssignedBy { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        [Required]
        public TaskTypeEnum TaskType { get; set; }

        [Required]
        public AssignedPlanStatus Status { get; set; } = AssignedPlanStatus.Assigned;


        public DateTime? FinalApprovedAt { get; set; }

        public string? FinalFeedback { get; set; }
    }
}
