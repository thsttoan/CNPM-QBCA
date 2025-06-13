using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class DifficultyLevel
    {
        [Key]
        public int DifficultyLevelID { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public int? SubjectID { get; set; }

        [Required(ErrorMessage = "Level Name is required.")]
        public string LevelName { get; set; }

        public Subject Subject { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}