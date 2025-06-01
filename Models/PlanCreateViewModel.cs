using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class PlanCreateViewModel
    {
        public int? PlanID { get; set; }

        [Required(ErrorMessage = "Plan name is required.")]
        [Display(Name = "Plan Name")]
        public string PlanName { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [Display(Name = "Subject")]
        public int SubjectID { get; set; }

        public List<Subject> AllSubjects { get; set; } = new List<Subject>();
        public List<DifficultyLevel> AllDifficultyLevels { get; set; } = new List<DifficultyLevel>();
        public List<User> AllManagers { get; set; } = new List<User>();

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

        [Display(Name = "Assign to Manager")]
        public int? AssignedManagerID { get; set; }
    }
}