using System.Collections.Generic;

namespace QBCA.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Role? Role { get; set; }
        public ICollection<TaskAssignment> TaskAssignmentsAssigned { get; set; } = new List<TaskAssignment>();
        public ICollection<TaskAssignment> TaskAssignmentsReceived { get; set; } = new List<TaskAssignment>();
        public ICollection<Subject> SubjectsCreated { get; set; } = new List<Subject>();
        public ICollection<ExamPlan> ExamPlansCreated { get; set; } = new List<ExamPlan>();
        public ICollection<ExamReview> ExamReviews { get; set; } = new List<ExamReview>();
        public ICollection<Question> QuestionsCreated { get; set; } = new List<Question>();
        public ICollection<SubmissionTable> SubmissionTablesCreated { get; set; } = new List<SubmissionTable>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Login> Logins { get; set; } = new List<Login>();
    }
}