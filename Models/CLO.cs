using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class CLO
    {
        [Key]
        public int CLOID { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public int? SubjectID { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "CLO Code is required.")]
        public string Code { get; set; }

        // Navigation properties
        public Subject Subject { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}