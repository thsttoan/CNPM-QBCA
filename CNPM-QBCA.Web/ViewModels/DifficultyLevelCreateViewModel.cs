using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class DifficultyLevelCreateViewModel
    {
        public int DifficultyLevelID { get; set; }

        public string? LevelName { get; set; }
        
        public List<string> LevelNames { get; set; } = new List<string>();

        [Required(ErrorMessage = "Subject is required.")]
        public int? SubjectID { get; set; }

        public List<int> SelectedQuestionIDs { get; set; } = new List<int>();

        public List<Question> AllQuestions { get; set; } = new List<Question>();
        public List<Subject> AllSubjects { get; set; } = new List<Subject>();
    }
}