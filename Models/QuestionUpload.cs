using System;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class QuestionUpload
    {
        [Key]
        public int UploadID { get; set; }
        public int UserID { get; set; }
        public int QuestionID { get; set; }
        public DateTime UploadTime { get; set; }

        public User User { get; set; }
        public Question Question { get; set; }
    }
}