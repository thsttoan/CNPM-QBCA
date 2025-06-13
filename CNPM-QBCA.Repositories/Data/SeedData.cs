using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QBCA.Models;
using System;
using System.Linq;

namespace QBCA.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Seed Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { RoleName = "R&D Staff" },
                        new Role { RoleName = "Head of Department" },
                        new Role { RoleName = "Subject Leader" },
                        new Role { RoleName = "Lecturer" },
                        new Role { RoleName = "Head of Examination Department" }
                    );
                    context.SaveChanges();
                }

                // Seed Users
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        
                    );
                    context.SaveChanges();
                }

                // Seed Subjects
                if (!context.Subjects.Any())
                {
                    context.Subjects.AddRange(
                        new Subject
                        {
                            SubjectName = "English 1",
                            CreatedBy = 1,
                            CreatedAt = DateTime.Now,
                            Status = "Active"
                        },
                        new Subject
                        {
                            SubjectName = "English 2",
                            CreatedBy = 1,
                            CreatedAt = DateTime.Now,
                            Status = "Active"
                        }
                    );
                    context.SaveChanges();
                }

                // Seed DifficultyLevels
                if (!context.DifficultyLevels.Any())
                {
                    context.DifficultyLevels.AddRange(
                        new DifficultyLevel { SubjectID = 1, LevelName = "Easy" },
                        new DifficultyLevel { SubjectID = 1, LevelName = "Medium" },
                        new DifficultyLevel { SubjectID = 1, LevelName = "Hard" }
                    );
                    context.SaveChanges();
                }

                // Seed CLOs
                if (!context.CLOs.Any())
                {
                    context.CLOs.AddRange(
                        new CLO { SubjectID = 1, Description = "CLO 1 for English 1", Code = "CLO1" },
                        new CLO { SubjectID = 1, Description = "CLO 2 for English 1", Code = "CLO2" }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}