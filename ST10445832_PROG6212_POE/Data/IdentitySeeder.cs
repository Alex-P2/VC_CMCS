// References
// https://medium.com/@roshanj100/users-and-roles-seeding-in-asp-net-core-identity-with-entity-framework-core-a-step-by-step-guide-28e6f76a18db

using Microsoft.AspNetCore.Identity;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

            var roles = new List<string>
            {
                "Lecturer",
                "Approver",
                "Programme Coordinator",
                "Academic Manager",
                "HR"
            };

            // create roles
            foreach (var role in roles)
            {
                // create if role doesn't exist
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // create sample users
            var sampleLecturer = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Smith",
                UserName = "JohnSmith",
                Email = "john@smith.co.za",
                HourlyRate = 500
            };

            if (await userManager.FindByEmailAsync(sampleLecturer.Email) is null)
            {
                var res = await userManager.CreateAsync(sampleLecturer, "@Password1");
                if (res.Succeeded)
                {   // assign role
                    await userManager.AddToRoleAsync(sampleLecturer, "Lecturer");
                    // create bank account
                    var user = await userManager.FindByEmailAsync(sampleLecturer.Email);
                    var bankAccount = new BankAccount
                    {
                        LecturerID = user.Id,
                        AccountHolder = $"{user.FirstName} {user.LastName}",
                        AccountNumber = "102391354987",
                        BranchCode = "47500",
                        BankName = "Capitec Bank"
                    };
                    var bankAccountRes = await dbContext.BankAccounts.AddAsync(bankAccount);
                    await dbContext.SaveChangesAsync();
                }
            }

            var sampleProgrammeCoordinator = new ApplicationUser
            {
                FirstName = "Jane",
                LastName = "Doe",
                UserName = "JaneDoe",
                Email = "jane@doe.co.za"
            };

            if (await userManager.FindByEmailAsync(sampleProgrammeCoordinator.Email) is null)
            {
                var res = await userManager.CreateAsync(sampleProgrammeCoordinator, "@Password1");
                if (res.Succeeded)
                {   // assign roles
                    await userManager.AddToRoleAsync(sampleProgrammeCoordinator, "Approver");
                    await userManager.AddToRoleAsync(sampleProgrammeCoordinator, "Programme Coordinator");
                }
            }

            var sampleAcademicManager = new ApplicationUser
            {
                FirstName = "Sam",
                LastName = "IAm",
                UserName = "SamIam",
                Email = "sam@iam.co.za"
            };

            if (await userManager.FindByEmailAsync(sampleAcademicManager.Email) is null)
            {
                var res = await userManager.CreateAsync(sampleAcademicManager, "@Password1");
                if (res.Succeeded)
                {   // assign role
                    await userManager.AddToRoleAsync(sampleAcademicManager, "Approver");
                    await userManager.AddToRoleAsync(sampleAcademicManager, "Academic Manager");
                }
            }

            var sampleHR = new ApplicationUser
            {
                FirstName = "Alex",
                LastName = "Pludd",
                UserName = "AlexPludd",
                Email = "alex@gmail.com"
            };

            if (await userManager.FindByEmailAsync(sampleHR.Email) is null)
            {
                var res = await userManager.CreateAsync(sampleHR, "@Password1");
                if (res.Succeeded)
                {   // assign role
                    await userManager.AddToRoleAsync(sampleHR, "HR");
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\