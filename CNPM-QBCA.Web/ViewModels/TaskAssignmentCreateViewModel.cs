using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class TaskAssignmentCreateViewModel
    {
        public int AssignmentID { get; set; }

        // Liên kết với ExamPlan
        [Display(Name = "Exam Plan")]
        [Required(ErrorMessage = "Exam Plan is required.")]
        public int? ExamPlanID { get; set; }

        // Liên kết với Distribution
        [Display(Name = "Exam Plan Distribution")]
        [Required(ErrorMessage = "Distribution is required.")]
        public int? DistributionID { get; set; }

        [Required(ErrorMessage = "Assignee is required.")]
        [Display(Name = "Assign To")]
        public int AssignedTo { get; set; }

        // Vai trò (Reviewer, Creator, Manager,...)
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Task type is required.")]
        [Display(Name = "Task Type")]
        public string TaskType { get; set; }

        // Mô tả nhiệm vụ
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [Required(ErrorMessage = "Due Date is required.")]
        public DateTime? DueDate { get; set; }

        // Trường này sẽ được gán ở controller dựa vào người đăng nhập
        public int AssignedBy { get; set; }

        // Danh sách hỗ trợ dropdown
        public List<User> AllUsers { get; set; } = new List<User>();

        public List<string> TaskTypeOptions { get; set; } = new List<string>
        {
            "Review", "Create Question", "Approve Plan"
        };

        public List<string> StatusOptions { get; set; } = new List<string>
        {
            "Pending", "In Progress", "Completed"
        };

        public List<string> RoleOptions { get; set; } = new List<string>
        {
            "Reviewer", "Creator", "Manager"
        };

        public List<ExamPlan> AvailableExamPlans { get; set; } = new List<ExamPlan>();
        public List<ExamPlanDistribution> AvailableDistributions { get; set; } = new List<ExamPlanDistribution>();

    }
}
