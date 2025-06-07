using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class TaskAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        // Tham chiếu đến kế hoạch đề
        public int ExamPlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        // Ai giao nhiệm vụ
        public int AssignedBy { get; set; }
        public User Assigner { get; set; }

        // Ai nhận nhiệm vụ
        [Required]
        public int AssignedTo { get; set; }
        public User Assignee { get; set; }

        // Vai trò (ví dụ: Reviewer, Creator, Manager)
        public string Role { get; set; }

        // Loại công việc
        [Required]
        public string TaskType { get; set; }

        // Mô tả nhiệm vụ
        public string Description { get; set; }

        // Trạng thái nhiệm vụ
        [Required]
        public string Status { get; set; }

        public DateTime AssignedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}