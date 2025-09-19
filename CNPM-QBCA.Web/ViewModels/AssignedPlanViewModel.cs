using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using QBCA.Models;

namespace QBCA.ViewModels
{
    public class AssignedPlanViewModel
    {
        public int AssignedPlanID { get; set; }

        // Thông tin hiển thị
        public string SubjectName { get; set; }
        public string AssignedToName { get; set; }
        public string DistributionStatus { get; set; }

        [Required]
        [Display(Name = "Exam Plan")]
        public int ExamPlanID { get; set; }

        [Required]
        [Display(Name = "Distribution")]
        public int DistributionID { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public int AssignedToID { get; set; }

        [Display(Name = "Assigned By")]
        public int AssignedByID { get; set; }

        public string AssignedByName { get; set; }

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Deadline")]
        public DateTime Deadline { get; set; }
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required]
        [Display(Name = "Task Type")]
        public TaskTypeEnum TaskType { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AssignedPlanStatus? Status { get; set; } = AssignedPlanStatus.Assigned;

        // Dropdown enums
        public IEnumerable<SelectListItem> TaskTypeOptions =>
            Enum.GetValues(typeof(TaskTypeEnum))
                .Cast<TaskTypeEnum>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.GetDisplayName()
                });

        public IEnumerable<SelectListItem> StatusOptions =>
            Enum.GetValues(typeof(AssignedPlanStatus))
                .Cast<AssignedPlanStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                });

        // Dữ liệu dropdown
        public List<ExamPlan> AllExamPlans { get; set; } = new();
        public List<ExamPlanDistribution> AllDistributions { get; set; } = new();
        public List<User> AllLecturers { get; set; } = new();
        public List<DistributionDisplayViewModel> DisplayDistributions { get; set; } = new();

    }

    // Extension cho Display(Name)
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                 .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? value.ToString();
        }
    }
    public class DistributionDisplayViewModel
    {
        [Required(ErrorMessage = "Please select a distribution")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid distribution")]
        public int? DistributionID { get; set; }

        public string DisplayName { get; set; }
    }

}
