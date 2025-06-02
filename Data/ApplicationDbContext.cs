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
        public DbSet<Login> Logins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CLO> CLOs { get; set; }
        public DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamPlan> ExamPlans { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamReview> ExamReviews { get; set; }
        public DbSet<SubmissionTable> SubmissionTables { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<DuplicateCheckResult> DuplicateCheckResults { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanDistribution> PlanDistributions { get; set; }

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

            // User - ExamPlansCreated
            modelBuilder.Entity<ExamPlan>()
                .HasOne(e => e.Creator)
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

            // User - Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID)
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
                .HasForeignKey(eq => eq.PlanID)
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

            // SubmissionTable
            modelBuilder.Entity<SubmissionTable>()
                .HasOne(st => st.ExamPlan)
                .WithMany(ep => ep.SubmissionTables)
                .HasForeignKey(st => st.PlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // Plan - Subject
            modelBuilder.Entity<Plan>()
                .HasOne(p => p.Subject)
                .WithMany(s => s.Plans)
                .HasForeignKey(p => p.SubjectID)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanDistribution - Plan
            modelBuilder.Entity<PlanDistribution>()
                .HasOne(pd => pd.Plan)
                .WithMany(p => p.Distributions)
                .HasForeignKey(pd => pd.PlanID)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanDistribution - DifficultyLevel
            modelBuilder.Entity<PlanDistribution>()
                .HasOne(pd => pd.DifficultyLevel)
                .WithMany()
                .HasForeignKey(pd => pd.DifficultyLevelID)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanDistribution - AssignedManager (User)
            modelBuilder.Entity<PlanDistribution>()
                .HasOne(pd => pd.AssignedManager)
                .WithMany()
                .HasForeignKey(pd => pd.AssignedManagerID)
                .OnDelete(DeleteBehavior.Restrict);

            // PlanDistribution.Status NOT NULL 
            modelBuilder.Entity<PlanDistribution>()
                .Property(pd => pd.Status)
                .HasDefaultValue("Assigned")
                .IsRequired();

            // Question - OPTIONAL: Không đặt IsRequired cho navigation/collection
            // Collections không cần mapping thêm nếu không có config đặc biệt
        }
    }
}