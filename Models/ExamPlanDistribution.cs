using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamPlanDistribution
    {
        [Key]
        public int DistributionID { get; set; }

        [Required]
        public int PlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        [Required]
        public int DifficultyLevelID { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required]
        public int AssignedManagerRoleID { get; set; }
        public Role AssignedManagerRole { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be > 0")]
        public int NumberOfQuestions { get; set; }

        [Required]
        public string Status { get; set; } = "Assigned";
    }
}