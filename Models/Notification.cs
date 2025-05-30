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

        public User User { get; set; }
    }
}