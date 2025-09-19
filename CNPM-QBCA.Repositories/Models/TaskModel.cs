using QBCA.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM_QBCA.Models
{
    public class TaskModel
    {
        public int TaskModelId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        
        public int AssignedTo { get; set; }
        public virtual User Assignee { get; set; }

        
        public int CreatedBy { get; set; }
        public virtual User Creator { get; set; }

        
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public string AssignedBy { get; set; }

      
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed

       
        public string TaskType { get; set; } = "General";

       
        public int? NotificationID { get; set; }
        public virtual Notification? Notification { get; set; }

        public int? ExamPlanID { get; set; }
        public virtual ExamPlan? ExamPlan { get; set; }

        
        public int? DistributionID { get; set; }
        public virtual ExamPlanDistribution? Distribution { get; set; }

        
        public int? ExamReviewID { get; set; }
        public virtual ExamReview? ExamReview { get; set; }

        
        public int? MockExamID { get; set; }
        public virtual MockExam? MockExam { get; set; }


        public int? FeedbackID { get; set; }
        public virtual MockFeedback? Feedback { get; set; }
    }
}
