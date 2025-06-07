using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamPlanDistribution
    {
        [Key]
        public int DistributionID { get; set; }

        [Required]
        public int ExamPlanID { get; set; }
        public ExamPlan ExamPlan { get; set; }

        [Required]
        public int DifficultyLevelID { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required]
        public int NumberOfQuestions { get; set; }

        public int? AssignedManagerRoleID { get; set; }
        public Role AssignedManagerRole { get; set; }

        public string Status { get; set; }
    }
}