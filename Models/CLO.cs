using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class CLO
    {
        [Key]
        public int CLOID { get; set; }
        public int SubjectID { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public Subject Subject { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}