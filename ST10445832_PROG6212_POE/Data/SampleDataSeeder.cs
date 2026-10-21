using Microsoft.AspNetCore.Identity;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class SampleDataSeeder
    {
        public static async Task InitialiseSampleData(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (dbContext.ClaimStatuses.Count() == 0)
            {
                await dbContext.ClaimStatuses.AddRangeAsync(new ClaimStatus[] {
                    new ClaimStatus { StatusTitle = "Pending", StatusMessage = "Claim awaiting verification." },
                    new ClaimStatus { StatusTitle = "Verified", StatusMessage = "Claim has been verified and is awaiting approval."},
                    new ClaimStatus { StatusTitle = "Approved", StatusMessage = "Claim has been approved." },
                    new ClaimStatus { StatusTitle = "Rejected", StatusMessage = "Claim has been rejected." },
                    new ClaimStatus { StatusTitle = "Processed", StatusMessage = "Claim has been processed." }
                });
                await dbContext.SaveChangesAsync();
            }

            if (dbContext.Modules.Count() == 0)
            {
                await dbContext.Modules.AddRangeAsync(new Module[]
                {
                    new Module { ModuleCode = "INSY6212", ModuleName = "Project Management" },
                    new Module { ModuleCode = "INSY7213", ModuleName = "Advanced Databases" },
                    new Module { ModuleCode = "PROG6211", ModuleName = "Programming A" },
                    new Module { ModuleCode = "PROG6212", ModuleName = "Programming B" },
                    new Module { ModuleCode = "CLDV6211", ModuleName = "Cloud Development A" },
                    new Module { ModuleCode = "CLDV6212", ModuleName = "Cloud Development B" },
                    new Module { ModuleCode = "INSY6211", ModuleName = "Systems Analysis and Design" },
                    new Module { ModuleCode = "INSY6112", ModuleName = "Databases" },
                    new Module { ModuleCode = "MAPC5211", ModuleName = "Mathematics" },
                    new Module { ModuleCode = "PHYS5212", ModuleName = "Physics" },
                    new Module { ModuleCode = "ICAD5221", ModuleName = "Introduction to CAD" },
                });
                await dbContext.SaveChangesAsync();
            }

            if (dbContext.Courses.Count() == 0)
            {
                await dbContext.Courses.AddRangeAsync(new Course[]
                {
                    new Course { CourseCode = "BCAD", CourseName = "Software Development", QualificationType = "Degree" },
                    new Course { CourseCode = "CIVE", CourseName = "Civil Engineering", QualificationType = "Degree" }
                });
                await dbContext.SaveChangesAsync();
            }

            if (dbContext.CourseModules.Count() == 0)
            {
                await dbContext.CourseModules.AddRangeAsync(new CourseModule[]
                {
                    new CourseModule { CourseID = 1 , ModuleID = 1, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 2, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 3, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 4, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 5, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 6, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 7, CountSessions = 32 },
                    new CourseModule { CourseID = 1 , ModuleID = 8, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 1, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 2, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 3, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 5, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 7, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 9, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 10, CountSessions = 32 },
                    new CourseModule { CourseID = 2 , ModuleID = 11, CountSessions = 32 }
                });
                await dbContext.SaveChangesAsync();
            }

            if (dbContext.LecturerModules.Count() == 0)
            {
                var lecturer = await userManager.FindByEmailAsync("john@smith.co.za");
                var lecturerID = lecturer.Id;
                await dbContext.LecturerModules.AddRangeAsync(new LecturerModule[]
                {
                    new LecturerModule { CourseModuleID = 1, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 2, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 3, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 4, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 5, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 6, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 7, LecturerID = lecturerID },
                    new LecturerModule { CourseModuleID = 8, LecturerID = lecturerID }
                });
                await dbContext.SaveChangesAsync();
            }

            if (dbContext.ApproverCourses.Count() == 0)
            {
                var approver1 = await userManager.FindByEmailAsync("jane@doe.co.za");
                var approver2 = await userManager.FindByEmailAsync("sam@iam.co.za");

                await dbContext.ApproverCourses.AddRangeAsync(new ApproverCourse[]
                {
                    new ApproverCourse { CourseID = 1, ApproverID = approver1.Id },
                    new ApproverCourse { CourseID = 1, ApproverID = approver2.Id },
                    new ApproverCourse { CourseID = 2, ApproverID = approver1.Id },
                    new ApproverCourse { CourseID = 2, ApproverID = approver2.Id },
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\