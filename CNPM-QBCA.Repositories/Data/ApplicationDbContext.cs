using CNPM_QBCA.Models;
using Microsoft.EntityFrameworkCore;
using QBCA.Models;

namespace QBCA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<ExamApproveTask> ExamApproveTasks { get; set; }
        public DbSet<ReviewExam> ReviewExams { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CLO> CLOs { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AssignedPlan> AssignPlans { get; set; }
        public DbSet<ExamPlan> ExamPlans { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamReview> ExamReviews { get; set; }
        public DbSet<SubmissionTable> SubmissionTables { get; set; }
        public DbSet<SubmissionApproval> SubmissionApprovals { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<DuplicateCheckResult> DuplicateCheckResults { get; set; }
        public DbSet<ExamPlanDistribution> ExamPlanDistributions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<MockExam> MockExam { get; set; }
        public DbSet<MockExamAnswer> MockExamAnswer { get; set; }
        public DbSet<MockFeedback> MockFeedback { get; set; }
        public DbSet<MockQuestion> MockQuestion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TaskAssignment - Assigner
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Assigner)
                .WithMany(u => u.TaskAssignmentsAssigned)
                .HasForeignKey(t => t.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskAssignment - Assignee
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.TaskAssignmentsReceived)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskAssignment - ExamPlan
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.ExamPlan)
                .WithMany(ep => ep.TaskAssignments)
                .HasForeignKey(t => t.ExamPlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // TaskAssignment - ExamPlanDistribution
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Distribution)
                .WithMany(pd => pd.TaskAssignments)
                .HasForeignKey(t => t.DistributionID)
                .OnDelete(DeleteBehavior.Restrict);

            // DuplicateCheckResult - Question & SimilarQuestion
            modelBuilder.Entity<DuplicateCheckResult>()
                .HasOne(d => d.Question)
                .WithMany(q => q.DuplicateCheckResults)
                .HasForeignKey(d => d.QuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DuplicateCheckResult>()
                .HasOne(d => d.SimilarQuestion)
                .WithMany(q => q.SimilarQuestions)
                .HasForeignKey(d => d.SimilarQuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            // User - SubjectsCreated
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Creator)
                .WithMany(u => u.SubjectsCreated)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User - ExamPlansCreated (chỉ dùng 1 navigation property duy nhất: CreatedByUser)
            modelBuilder.Entity<ExamPlan>()
                .HasOne(e => e.CreatedByUser)
                .WithMany(u => u.ExamPlansCreated)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User - QuestionsCreated
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Creator)
                .WithMany(u => u.QuestionsCreated)
                .HasForeignKey(q => q.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User - ExamReviews
            modelBuilder.Entity<ExamReview>()
                .HasOne(er => er.Reviewer)
                .WithMany(u => u.ExamReviews)
                .HasForeignKey(er => er.ReviewerID)
                .OnDelete(DeleteBehavior.Restrict);

            // User - SubmissionTablesCreated
            modelBuilder.Entity<SubmissionTable>()
                .HasOne(s => s.Creator)
                .WithMany(u => u.SubmissionTablesCreated)
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Notifications (receiver)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // User - NotificationsCreated (creator)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(n => n.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Login - User & Role
            modelBuilder.Entity<Login>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logins)
                .HasForeignKey(l => l.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Login>()
                .HasOne(l => l.Role)
                .WithMany()
                .HasForeignKey(l => l.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject - Questions, CLOs, DifficultyLevels, ExamPlans
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Subject)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CLO>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.CLOs)
                .HasForeignKey(c => c.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DifficultyLevel>()
                .HasOne(dl => dl.Subject)
                .WithMany(s => s.DifficultyLevels)
                .HasForeignKey(dl => dl.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamPlan>()
                .HasOne(ep => ep.Subject)
                .WithMany(s => s.ExamPlans)
                .HasForeignKey(ep => ep.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            // Question - CLO, DifficultyLevel
            modelBuilder.Entity<Question>()
                .HasOne(q => q.CLO)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CLOID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.DifficultyLevel)
                .WithMany(dl => dl.Questions)
                .HasForeignKey(q => q.DifficultyLevelID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamQuestion
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.ExamPlan)
                .WithMany(ep => ep.ExamQuestions)
                .HasForeignKey(eq => eq.ExamPlanID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamReview
            modelBuilder.Entity<ExamReview>()
                .HasOne(er => er.ExamQuestion)
                .WithMany(eq => eq.ExamReviews)
                .HasForeignKey(er => er.ExamQuestionID)
                .OnDelete(DeleteBehavior.Restrict);

            // SubmissionTable - ExamPlan
            modelBuilder.Entity<SubmissionTable>()
                .HasOne(st => st.ExamPlan)
                .WithMany(ep => ep.SubmissionTables)
                .HasForeignKey(st => st.PlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // SubmissionTable - Approver (User)
            modelBuilder.Entity<SubmissionTable>()
                .HasOne(st => st.Approver)
                .WithMany()
                .HasForeignKey(st => st.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Question - SubmissionTable
            modelBuilder.Entity<Question>()
                .HasOne(q => q.SubmissionTable)
                .WithMany(st => st.Questions)
                .HasForeignKey(q => q.SubmissionID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPlanDistribution - ExamPlan
            modelBuilder.Entity<ExamPlanDistribution>()
                .HasOne(pd => pd.ExamPlan)
                .WithMany(ep => ep.Distributions)
                .HasForeignKey(pd => pd.ExamPlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPlanDistribution - DifficultyLevel
            modelBuilder.Entity<ExamPlanDistribution>()
                .HasOne(pd => pd.DifficultyLevel)
                .WithMany()
                .HasForeignKey(pd => pd.DifficultyLevelID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPlanDistribution - AssignedManagerRole (Role)
            modelBuilder.Entity<ExamPlanDistribution>()
                .HasOne(pd => pd.AssignedManagerRole)
                .WithMany()
                .HasForeignKey(pd => pd.AssignedManagerRoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamPlanDistribution.Status NOT NULL 
            modelBuilder.Entity<ExamPlanDistribution>()
                .Property(pd => pd.Status)
                .HasDefaultValue("Assigned")
                .IsRequired();

            // 1-1: Exam - ExamPlanDistribution
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.ExamPlanDistribution)
                .WithOne(pd => pd.Exam)
                .HasForeignKey<Exam>(e => e.DistributionID)
                .OnDelete(DeleteBehavior.Restrict);

            // n-1: Exam - Submitter
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Submitter)
                .WithMany()
                .HasForeignKey(e => e.SubmittedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-n: Exam - ExamQuestions
            modelBuilder.Entity<Exam>()
                .HasMany(e => e.ExamQuestions)
                .WithOne(eq => eq.Exam)
                .HasForeignKey(eq => eq.ExamID)
                .OnDelete(DeleteBehavior.Cascade);
            

            

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Feedback)
                .WithMany(me => me.Tasks)
                .HasForeignKey(t => t.FeedbackID)
                .OnDelete(DeleteBehavior.Restrict);


            
            // SubmissionApproval - SubmissionTable
            modelBuilder.Entity<SubmissionApproval>()
                .HasOne(sa => sa.SubmissionTable)
                .WithMany(st => st.SubmissionApprovals)
                .HasForeignKey(sa => sa.SubmissionTableID)
                .OnDelete(DeleteBehavior.Restrict);

            // SubmissionApproval - Approver (User)
            modelBuilder.Entity<SubmissionApproval>()
                .HasOne(sa => sa.Approver)
                .WithMany(u => u.SubmissionApprovals)
                .HasForeignKey(sa => sa.ApprovedBy);



            modelBuilder.Entity<AssignedPlan>()
                .HasOne(ap => ap.ExamPlan)
                .WithMany()
                .HasForeignKey(ap => ap.ExamPlanID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignedPlan>()
                .HasOne(ap => ap.Distribution)
                .WithMany()
                .HasForeignKey(ap => ap.DistributionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedPlan>()
                .HasOne(ap => ap.AssignedTo)
                .WithMany()
                .HasForeignKey(ap => ap.AssignedToID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedPlan>()
                .HasOne(ap => ap.AssignedBy)
                .WithMany()
                .HasForeignKey(ap => ap.AssignedByID)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<TaskModel>()
               .HasOne(t => t.Assignee)
               .WithMany(u => u.TasksAssigned)
               .HasForeignKey(t => t.AssignedTo)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.TasksCreated)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.ExamPlan)
                .WithMany()
                .HasForeignKey(t => t.ExamPlanID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Distribution)
                .WithMany()
                .HasForeignKey(t => t.DistributionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Notification)
                .WithMany()
                .HasForeignKey(t => t.NotificationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.ExamReview)
                .WithMany(me => me.Tasks)
                .HasForeignKey(t => t.ExamReviewID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.MockExam)
                .WithMany(me => me.Tasks)
                .HasForeignKey(t => t.MockExamID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Feedback)
                .WithMany(me => me.Tasks)
                .HasForeignKey(t => t.FeedbackID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MockExamAnswer>()
                .HasOne(a => a.MockExam)
                .WithMany(me => me.Answers)
                .HasForeignKey(a => a.MockExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MockFeedback>()
                .HasOne(f => f.MockExam)
                .WithMany(me => me.Feedbacks)
                .HasForeignKey(f => f.MockExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MockQuestion>()
                .HasOne(q => q.MockExam)
                .WithMany(me => me.Questions)
                .HasForeignKey(q => q.MockExamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Distribution)
                .WithMany() // hoặc .WithMany(d => d.TaskAssignments) nếu có
                .HasForeignKey(t => t.DistributionID);

            // Approve
            modelBuilder.Entity<ExamApproveTask>()
                .HasOne(e => e.AssignedBy)
                .WithMany()
                .HasForeignKey(e => e.AssignedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamApproveTask>()
                .HasOne(e => e.AssignedTo)
                .WithMany()
                .HasForeignKey(e => e.AssignedToUserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Nếu có Exam:
            modelBuilder.Entity<ExamApproveTask>()
                .HasOne(e => e.Exam)
                .WithMany()
                .HasForeignKey(e => e.ExamID);



        }


    }
}
