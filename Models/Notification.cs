using System;

namespace QBCA.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string RelatedEntityType { get; set; }
        public int? RelatedEntityID { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }            // ID của người tạo thông báo

        // Navigation properties
        public User User { get; set; }                // Người nhận thông báo
        public User CreatedByUser { get; set; }       // Người tạo thông báo (nếu cần include thông tin)
    }
}