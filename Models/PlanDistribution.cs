using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class PlanDistribution
    {
        [Key]
        public int DistributionID { get; set; }

        [Required]
        public int PlanID { get; set; }
        public Plan Plan { get; set; }

        [Required]
        public int DifficultyLevelID { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        public int? AssignedManagerID { get; set; }
        public User AssignedManager { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be > 0")]
        public int NumberOfQuestions { get; set; }

        [Required]
        public string Status { get; set; } = "Assigned";

        public PlanDistribution()
        {
            Status ??= "Assigned";
        }
    }
}