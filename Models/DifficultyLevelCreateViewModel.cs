using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class DifficultyLevelCreateViewModel
    {
        public int DifficultyLevelID { get; set; }

        [Required(ErrorMessage = "Level Name is required.")]
        public string LevelName { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public int? SubjectID { get; set; }

        public List<int> SelectedQuestionIDs { get; set; } = new List<int>();

        public List<Question> AllQuestions { get; set; } = new List<Question>();
        public List<Subject> AllSubjects { get; set; } = new List<Subject>();
    }
}