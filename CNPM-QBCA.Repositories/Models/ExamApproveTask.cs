using QBCA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBCA.Models
{
    public class ExamApproveTask
    {
        [Key]
        public int ExamApproveTaskID { get; set; }

        public int ExamID { get; set; }
        public Exam Exam { get; set; }

        public int AssignedToUserID { get; set; }
        [ForeignKey("AssignedToUserID")]
        public User AssignedTo { get; set; }

        public int AssignedByUserID { get; set; }
        [ForeignKey("AssignedByUserID")]
        public User AssignedBy { get; set; }


        public string Status { get; set; } // Pending, Approved, Rejected, InReview
        public string Feedback { get; set; } // Nhận xét của người phê duyệt

        public DateTime AssignedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public bool IsFinalVersion { get; set; } // Nếu đây là bản duyệt cuối

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
