using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM_QBCA.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    LoginID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.LoginID);
                    table.ForeignKey(
                        name: "FK_Logins_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectID);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CLOs",
                columns: table => new
                {
                    CLOID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLOs", x => x.CLOID);
                    table.ForeignKey(
                        name: "FK_CLOs_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DifficultyLevels",
                columns: table => new
                {
                    DifficultyLevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifficultyLevels", x => x.DifficultyLevelID);
                    table.ForeignKey(
                        name: "FK_DifficultyLevels_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamPlans",
                columns: table => new
                {
                    ExamPlanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPlans", x => x.ExamPlanID);
                    table.ForeignKey(
                        name: "FK_ExamPlans_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPlans_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamPlanDistributions",
                columns: table => new
                {
                    DistributionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevelID = table.Column<int>(type: "int", nullable: false),
                    NumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    AssignedManagerRoleID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Assigned")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamPlanDistributions", x => x.DistributionID);
                    table.ForeignKey(
                        name: "FK_ExamPlanDistributions_DifficultyLevels_DifficultyLevelID",
                        column: x => x.DifficultyLevelID,
                        principalTable: "DifficultyLevels",
                        principalColumn: "DifficultyLevelID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPlanDistributions_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamPlanDistributions_Roles_AssignedManagerRoleID",
                        column: x => x.AssignedManagerRoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionTables",
                columns: table => new
                {
                    SubmissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReviewerComment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DuplicateRate = table.Column<double>(type: "float", nullable: true),
                    DuplicateReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionTables", x => x.SubmissionID);
                    table.ForeignKey(
                        name: "FK_SubmissionTables_ExamPlans_PlanID",
                        column: x => x.PlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionTables_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionTables_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignPlans",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false),
                    DistributionID = table.Column<int>(type: "int", nullable: false),
                    AssignedToID = table.Column<int>(type: "int", nullable: false),
                    AssignedByID = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FinalApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalFeedback = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignPlans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AssignPlans_ExamPlanDistributions_DistributionID",
                        column: x => x.DistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignPlans_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignPlans_Users_AssignedByID",
                        column: x => x.AssignedByID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignPlans_Users_AssignedToID",
                        column: x => x.AssignedToID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    ExamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistributionID = table.Column<int>(type: "int", nullable: false),
                    SubmitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.ExamID);
                    table.ForeignKey(
                        name: "FK_Exams_ExamPlanDistributions_DistributionID",
                        column: x => x.DistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_Users_SubmittedBy",
                        column: x => x.SubmittedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewExams",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false),
                    DistributionID = table.Column<int>(type: "int", nullable: false),
                    ReviewerID = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewExams", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_ReviewExams_ExamPlanDistributions_DistributionID",
                        column: x => x.DistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewExams_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewExams_Users_ReviewerID",
                        column: x => x.ReviewerID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAssignments",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false),
                    DistributionID = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamPlanDistributionDistributionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_TaskAssignments_ExamPlanDistributions_DistributionID",
                        column: x => x.DistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignments_ExamPlanDistributions_ExamPlanDistributionDistributionID",
                        column: x => x.ExamPlanDistributionDistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID");
                    table.ForeignKey(
                        name: "FK_TaskAssignments_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignments_Users_AssignedBy",
                        column: x => x.AssignedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignments_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectID = table.Column<int>(type: "int", nullable: false),
                    CLOID = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevelID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionID = table.Column<int>(type: "int", nullable: true),
                    ExamPlanDistributionDistributionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Questions_CLOs_CLOID",
                        column: x => x.CLOID,
                        principalTable: "CLOs",
                        principalColumn: "CLOID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_DifficultyLevels_DifficultyLevelID",
                        column: x => x.DifficultyLevelID,
                        principalTable: "DifficultyLevels",
                        principalColumn: "DifficultyLevelID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_ExamPlanDistributions_ExamPlanDistributionDistributionID",
                        column: x => x.ExamPlanDistributionDistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID");
                    table.ForeignKey(
                        name: "FK_Questions_Subjects_SubjectID",
                        column: x => x.SubjectID,
                        principalTable: "Subjects",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_SubmissionTables_SubmissionID",
                        column: x => x.SubmissionID,
                        principalTable: "SubmissionTables",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionApprovals",
                columns: table => new
                {
                    SubmissionApprovalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionTableID = table.Column<int>(type: "int", nullable: false),
                    AssignedPlanID = table.Column<int>(type: "int", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionApprovals", x => x.SubmissionApprovalID);
                    table.ForeignKey(
                        name: "FK_SubmissionApprovals_AssignPlans_AssignedPlanID",
                        column: x => x.AssignedPlanID,
                        principalTable: "AssignPlans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionApprovals_SubmissionTables_SubmissionTableID",
                        column: x => x.SubmissionTableID,
                        principalTable: "SubmissionTables",
                        principalColumn: "SubmissionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubmissionApprovals_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamApproveTasks",
                columns: table => new
                {
                    ExamApproveTaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamID = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserID = table.Column<int>(type: "int", nullable: false),
                    AssignedByUserID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFinalVersion = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamID1 = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamApproveTasks", x => x.ExamApproveTaskID);
                    table.ForeignKey(
                        name: "FK_ExamApproveTasks_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ExamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamApproveTasks_Exams_ExamID1",
                        column: x => x.ExamID1,
                        principalTable: "Exams",
                        principalColumn: "ExamID");
                    table.ForeignKey(
                        name: "FK_ExamApproveTasks_Users_AssignedByUserID",
                        column: x => x.AssignedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamApproveTasks_Users_AssignedToUserID",
                        column: x => x.AssignedToUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamApproveTasks_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "MockExam",
                columns: table => new
                {
                    MockExamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentID = table.Column<int>(type: "int", nullable: false),
                    LecturerID = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnswersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockExam", x => x.MockExamID);
                    table.ForeignKey(
                        name: "FK_MockExam_TaskAssignments_AssignmentID",
                        column: x => x.AssignmentID,
                        principalTable: "TaskAssignments",
                        principalColumn: "AssignmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DuplicateCheckResults",
                columns: table => new
                {
                    CheckID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    SimilarQuestionID = table.Column<int>(type: "int", nullable: false),
                    SimilarityScore = table.Column<double>(type: "float", nullable: false),
                    CheckType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuplicateCheckResults", x => x.CheckID);
                    table.ForeignKey(
                        name: "FK_DuplicateCheckResults_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuplicateCheckResults_Questions_SimilarQuestionID",
                        column: x => x.SimilarQuestionID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    ExamQuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamID = table.Column<int>(type: "int", nullable: false),
                    ExamPlanID = table.Column<int>(type: "int", nullable: false),
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => x.ExamQuestionID);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ExamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MockExamAnswer",
                columns: table => new
                {
                    MockExamAnswerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockExamId = table.Column<int>(type: "int", nullable: false),
                    LecturerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockExamAnswer", x => x.MockExamAnswerID);
                    table.ForeignKey(
                        name: "FK_MockExamAnswer_MockExam_MockExamId",
                        column: x => x.MockExamId,
                        principalTable: "MockExam",
                        principalColumn: "MockExamID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MockFeedback",
                columns: table => new
                {
                    MockFeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockExamId = table.Column<int>(type: "int", nullable: false),
                    LecturerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockFeedback", x => x.MockFeedbackID);
                    table.ForeignKey(
                        name: "FK_MockFeedback_MockExam_MockExamId",
                        column: x => x.MockExamId,
                        principalTable: "MockExam",
                        principalColumn: "MockExamID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MockQuestion",
                columns: table => new
                {
                    MockQuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MockExamId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockQuestion", x => x.MockQuestionID);
                    table.ForeignKey(
                        name: "FK_MockQuestion_MockExam_MockExamId",
                        column: x => x.MockExamId,
                        principalTable: "MockExam",
                        principalColumn: "MockExamID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamQuestionID = table.Column<int>(type: "int", nullable: false),
                    ReviewerID = table.Column<int>(type: "int", nullable: false),
                    ReviewResult = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_ExamReviews_ExamQuestions_ExamQuestionID",
                        column: x => x.ExamQuestionID,
                        principalTable: "ExamQuestions",
                        principalColumn: "ExamQuestionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamReviews_Users_ReviewerID",
                        column: x => x.ReviewerID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskModels",
                columns: table => new
                {
                    TaskModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedTo = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationID = table.Column<int>(type: "int", nullable: true),
                    ExamPlanID = table.Column<int>(type: "int", nullable: true),
                    DistributionID = table.Column<int>(type: "int", nullable: true),
                    ExamReviewID = table.Column<int>(type: "int", nullable: true),
                    MockExamID = table.Column<int>(type: "int", nullable: true),
                    FeedbackID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskModels", x => x.TaskModelId);
                    table.ForeignKey(
                        name: "FK_TaskModels_ExamPlanDistributions_DistributionID",
                        column: x => x.DistributionID,
                        principalTable: "ExamPlanDistributions",
                        principalColumn: "DistributionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_ExamPlans_ExamPlanID",
                        column: x => x.ExamPlanID,
                        principalTable: "ExamPlans",
                        principalColumn: "ExamPlanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_ExamReviews_ExamReviewID",
                        column: x => x.ExamReviewID,
                        principalTable: "ExamReviews",
                        principalColumn: "ReviewID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_MockExam_MockExamID",
                        column: x => x.MockExamID,
                        principalTable: "MockExam",
                        principalColumn: "MockExamID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_MockFeedback_FeedbackID",
                        column: x => x.FeedbackID,
                        principalTable: "MockFeedback",
                        principalColumn: "MockFeedbackID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_Notifications_NotificationID",
                        column: x => x.NotificationID,
                        principalTable: "Notifications",
                        principalColumn: "NotificationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskModels_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignPlans_AssignedByID",
                table: "AssignPlans",
                column: "AssignedByID");

            migrationBuilder.CreateIndex(
                name: "IX_AssignPlans_AssignedToID",
                table: "AssignPlans",
                column: "AssignedToID");

            migrationBuilder.CreateIndex(
                name: "IX_AssignPlans_DistributionID",
                table: "AssignPlans",
                column: "DistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_AssignPlans_ExamPlanID",
                table: "AssignPlans",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_CLOs_SubjectID",
                table: "CLOs",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultyLevels_SubjectID",
                table: "DifficultyLevels",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateCheckResults_QuestionID",
                table: "DuplicateCheckResults",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateCheckResults_SimilarQuestionID",
                table: "DuplicateCheckResults",
                column: "SimilarQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamApproveTasks_AssignedByUserID",
                table: "ExamApproveTasks",
                column: "AssignedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamApproveTasks_AssignedToUserID",
                table: "ExamApproveTasks",
                column: "AssignedToUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamApproveTasks_ExamID",
                table: "ExamApproveTasks",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamApproveTasks_ExamID1",
                table: "ExamApproveTasks",
                column: "ExamID1");

            migrationBuilder.CreateIndex(
                name: "IX_ExamApproveTasks_UserID",
                table: "ExamApproveTasks",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPlanDistributions_AssignedManagerRoleID",
                table: "ExamPlanDistributions",
                column: "AssignedManagerRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPlanDistributions_DifficultyLevelID",
                table: "ExamPlanDistributions",
                column: "DifficultyLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPlanDistributions_ExamPlanID",
                table: "ExamPlanDistributions",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPlans_CreatedBy",
                table: "ExamPlans",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ExamPlans_SubjectID",
                table: "ExamPlans",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamID",
                table: "ExamQuestions",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamPlanID",
                table: "ExamQuestions",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_QuestionID",
                table: "ExamQuestions",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamReviews_ExamQuestionID",
                table: "ExamReviews",
                column: "ExamQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_ExamReviews_ReviewerID",
                table: "ExamReviews",
                column: "ReviewerID");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_DistributionID",
                table: "Exams",
                column: "DistributionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SubmittedBy",
                table: "Exams",
                column: "SubmittedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_RoleID",
                table: "Logins",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_UserID",
                table: "Logins",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MockExam_AssignmentID",
                table: "MockExam",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_MockExamAnswer_MockExamId",
                table: "MockExamAnswer",
                column: "MockExamId");

            migrationBuilder.CreateIndex(
                name: "IX_MockFeedback_MockExamId",
                table: "MockFeedback",
                column: "MockExamId");

            migrationBuilder.CreateIndex(
                name: "IX_MockQuestion_MockExamId",
                table: "MockQuestion",
                column: "MockExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedBy",
                table: "Notifications",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CLOID",
                table: "Questions",
                column: "CLOID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatedBy",
                table: "Questions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_DifficultyLevelID",
                table: "Questions",
                column: "DifficultyLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamPlanDistributionDistributionID",
                table: "Questions",
                column: "ExamPlanDistributionDistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubjectID",
                table: "Questions",
                column: "SubjectID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubmissionID",
                table: "Questions",
                column: "SubmissionID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewExams_DistributionID",
                table: "ReviewExams",
                column: "DistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewExams_ExamPlanID",
                table: "ReviewExams",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewExams_ReviewerID",
                table: "ReviewExams",
                column: "ReviewerID");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CreatedBy",
                table: "Subjects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionApprovals_ApprovedBy",
                table: "SubmissionApprovals",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionApprovals_AssignedPlanID",
                table: "SubmissionApprovals",
                column: "AssignedPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionApprovals_SubmissionTableID",
                table: "SubmissionApprovals",
                column: "SubmissionTableID");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionTables_ApprovedBy",
                table: "SubmissionTables",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionTables_CreatedBy",
                table: "SubmissionTables",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionTables_PlanID",
                table: "SubmissionTables",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_AssignedBy",
                table: "TaskAssignments",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_AssignedTo",
                table: "TaskAssignments",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_DistributionID",
                table: "TaskAssignments",
                column: "DistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_ExamPlanDistributionDistributionID",
                table: "TaskAssignments",
                column: "ExamPlanDistributionDistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_ExamPlanID",
                table: "TaskAssignments",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_AssignedTo",
                table: "TaskModels",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_CreatedBy",
                table: "TaskModels",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_DistributionID",
                table: "TaskModels",
                column: "DistributionID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_ExamPlanID",
                table: "TaskModels",
                column: "ExamPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_ExamReviewID",
                table: "TaskModels",
                column: "ExamReviewID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_FeedbackID",
                table: "TaskModels",
                column: "FeedbackID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_MockExamID",
                table: "TaskModels",
                column: "MockExamID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskModels_NotificationID",
                table: "TaskModels",
                column: "NotificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuplicateCheckResults");

            migrationBuilder.DropTable(
                name: "ExamApproveTasks");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "MockExamAnswer");

            migrationBuilder.DropTable(
                name: "MockQuestion");

            migrationBuilder.DropTable(
                name: "ReviewExams");

            migrationBuilder.DropTable(
                name: "SubmissionApprovals");

            migrationBuilder.DropTable(
                name: "TaskModels");

            migrationBuilder.DropTable(
                name: "AssignPlans");

            migrationBuilder.DropTable(
                name: "ExamReviews");

            migrationBuilder.DropTable(
                name: "MockFeedback");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "MockExam");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "TaskAssignments");

            migrationBuilder.DropTable(
                name: "CLOs");

            migrationBuilder.DropTable(
                name: "SubmissionTables");

            migrationBuilder.DropTable(
                name: "ExamPlanDistributions");

            migrationBuilder.DropTable(
                name: "DifficultyLevels");

            migrationBuilder.DropTable(
                name: "ExamPlans");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
