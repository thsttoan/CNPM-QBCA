using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class CLOCreateViewModel
    {
        public int CLOID { get; set; }

        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int? SubjectID { get; set; }

        public List<int> SelectedQuestionIDs { get; set; } = new List<int>();

        public List<Question> AllQuestions { get; set; } = new List<Question>();
        public List<Subject> AllSubjects { get; set; } = new List<Subject>();
    }
}