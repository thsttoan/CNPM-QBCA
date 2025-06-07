using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamPlanCreateViewModel
    {
        public int? ExamPlanID { get; set; }

        [Required(ErrorMessage = "Plan name is required.")]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [Display(Name = "Subject")]
        public int SubjectID { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<string> StatusOptions { get; set; } = new List<string>
        {
            "Pending",
            "Approved",
            "Rejected"
        };

        public List<Subject> AllSubjects { get; set; } = new List<Subject>();
        public List<DifficultyLevel> AllDifficultyLevels { get; set; } = new List<DifficultyLevel>();
        public List<Role> AllManagerRoles { get; set; } = new List<Role>();

        public List<PlanDistributionViewModel> Distributions { get; set; } = new List<PlanDistributionViewModel>();
    }

    public class PlanDistributionViewModel
    {
        public int? DistributionID { get; set; }

        [Required]
        [Display(Name = "Difficulty Level")]
        public int DifficultyLevelID { get; set; }

        [Required(ErrorMessage = "Number of questions is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number must be greater than 0.")]
        [Display(Name = "Number of Questions")]
        public int NumberOfQuestions { get; set; }

        [Required(ErrorMessage = "Manager Role is required.")]
        [Display(Name = "Manager Role")]
        public int? AssignedManagerRoleID { get; set; }
    }
}