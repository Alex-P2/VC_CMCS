// References
// https://learn.microsoft.com/en-us/aspnet/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST10445832_PROG6212_POE.Areas.HR.Models;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HR")]
    public class UserManagementController : Controller
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Identity user manager
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;


        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public UserManagementController(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        //=============================================================================================================================//
        /// <summary>
        /// Home view for user management
        /// </summary>
        /// <param name="query">Search query for user filter</param>
        public async Task<IActionResult> Index(string query)
        {
            // fetch users and their roles (excluding approver and admin role to avoid duplicate users)
            List<UserDetail> users = await _dbContext.Users.Join(_dbContext.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { User = user, UserRole = userRole })
                .Join(_dbContext.Roles,
                    userWithUserRole => userWithUserRole.UserRole.RoleId,
                    role => role.Id,
                    (userWithUserRole, role) => new { User = userWithUserRole, Role = role.Name })
                .Where(userWithRole => !userWithRole.Role.Equals("Approver") && !userWithRole.Role.Equals("Admin") && !userWithRole.Role.Equals("HR"))
                .Where(uwr => uwr.User.User.IsDeleted == 0)
                .Select(userWithRole => new UserDetail
                {
                    Id = userWithRole.User.User.Id,
                    UserName = userWithRole.User.User.UserName,
                    Email = userWithRole.User.User.Email,
                    Role = userWithRole.Role
                })
                .ToListAsync();

            // filter if required
            var filterRoles = !string.IsNullOrEmpty(query);
            var roleFilters = new string[] { "Academic Manager", "Lecturer", "Programme Coordinator" };
            var roleFilter = string.Empty;

            if (filterRoles && roleFilters.Contains(query))
            {
                roleFilter = query;
                users = users.Where(user => user.Role.Equals(query)).ToList();
            }
            else if (filterRoles)
            {
                users = users.Where(user => JsonConvert.SerializeObject(user).ToLower().Contains(query.ToLower())).ToList();
            }

            var viewModel = new UserManagementViewModel
            {
                Filtered = filterRoles,
                Filter = roleFilter,
                Query = query,
                Users = users
            };

            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Load view for creating a new user
        /// </summary>
        public async Task<IActionResult> Create()
        {
            var allModules = await _dbContext.Modules.Select(m => m.ModuleCode).ToListAsync();
            var allCourses = await _dbContext.Courses.Select(c => c.CourseCode).ToListAsync();
            var courseModules = await _dbContext.CourseModules
                .Select(cm => new CourseModuleWithCodes
                {
                    CourseCode = cm.Course.CourseCode,
                    CourseId = cm.CourseID,
                    ModuleCode = cm.Module.ModuleCode,
                    ModuleId = cm.ModuleID,
                    CourseModuleId = cm.CourseModuleID
                })
                .ToListAsync();
            var courses = await _dbContext.Courses
                .Select(c => new { c.CourseID, c.CourseCode })
                .ToListAsync();

            var courseTuples = courses.Select(c => (c.CourseID, c.CourseCode)).ToList();

            var viewModel = new CreateUserViewModel
            {
                Courses = courseTuples,
                CourseModules = courseModules
            };
            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="model">Model for user creation view</param>
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            var courseModuleIds = JsonConvert.DeserializeObject<List<int>>(model.ModulesJson); // modules for lecturer
            var courseIds = JsonConvert.DeserializeObject<List<int>>(model.CoursesJson); // courses for approvers

            Console.WriteLine(JsonConvert.SerializeObject(model));

            // validation
            if (!string.IsNullOrEmpty(model.Role) && model.Role.Equals("Lecturer"))
            {
                if (model.HourlyRate <= 0)
                {
                    ModelState.AddModelError("HourlyRate", "Hourly Rate must be greater than zero for Lecturers.");
                }

                if (courseModuleIds.Count < 1)
                {
                    ModelState.AddModelError("ModulesJson", "At least one module must be assigned to Lecturer.");
                }

                if (string.IsNullOrEmpty(model.BankName))
                {
                    ModelState.AddModelError("BankName", "Bank name is required");
                }

                if (string.IsNullOrEmpty(model.BranchCode))
                {
                    ModelState.AddModelError("BranchCode", "Branch code is required");
                }

                if (string.IsNullOrEmpty(model.AccountNumber))
                {
                    ModelState.AddModelError("AccountNumber", "Bank account number is required");
                }

                if (string.IsNullOrEmpty(model.AccountHolder))
                {
                    ModelState.AddModelError("AccountHolder", "Bank account holder name is required.");
                }
            }

            if (!string.IsNullOrEmpty(model.Role) && (model.Role.Equals("Academic Manager") || model.Role.Equals("Programme Coordinator")) && courseIds.Count < 1)
            {
                ModelState.AddModelError("CoursesJson", $"At least on course must be assigned to {(model.Role.Equals("Academic Manager") ? "an academic manager." : "a programme manager.")}");
            }

            if (!ModelState.IsValid)
            {
                model.CourseModules = await _dbContext.CourseModules
                    .Select(cm => new CourseModuleWithCodes
                    {
                        CourseModuleId = cm.CourseModuleID,
                        CourseCode = cm.Course.CourseCode,
                        ModuleId = cm.Module.ModuleID,
                        ModuleCode = cm.Module.ModuleCode,
                    })
                    .ToListAsync();

                var courses = await _dbContext.Courses
                    .Select(c => new { c.CourseID, c.CourseCode })
                    .ToListAsync();

                model.Courses = courses.Select(c => (c.CourseID, c.CourseCode)).ToList();

                return View(model);
            }

            // add user
            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = $"{model.FirstName}{model.LastName}",
                Email = model.Email,
                IsDeleted = 0
            };

            if (model.Role.Equals("Lecturer"))
            {
                newUser.HourlyRate = model.HourlyRate;
            }

            if (await _userManager.FindByEmailAsync(newUser.Email) is null) // email isn't used
            {
                var res = await _userManager.CreateAsync(newUser, model.Password);
                if (res.Succeeded)
                {
                    newUser = await _userManager.FindByEmailAsync(newUser.Email);
                    if (model.Role.Equals("Lecturer"))
                    {
                        await _userManager.AddToRoleAsync(newUser, "Lecturer");
                        newUser = await _userManager.FindByEmailAsync(newUser.Email);
                        // add modules and bank account
                        foreach (var id in courseModuleIds)
                        {
                            // get the course module id associated with the module code
                            var lecturerModule = new LecturerModule
                            {
                                LecturerID = newUser.Id,
                                CourseModuleID = id
                            };

                            var response = await _dbContext.LecturerModules.AddAsync(lecturerModule);
                        }

                        var bankAccount = new BankAccount
                        {
                            BankName = model.BankName,
                            BranchCode = model.BranchCode,
                            AccountHolder = model.AccountHolder,
                            AccountNumber = model.AccountNumber,
                            LecturerID = newUser.Id
                        };

                        await _dbContext.BankAccounts.AddAsync(bankAccount);
                    }
                    else if (model.Role.Equals("Academic Manager"))
                    {
                        await _userManager.AddToRoleAsync(newUser, "Approver");
                        await _userManager.AddToRoleAsync(newUser, "Academic Manager");
                    }
                    else if (model.Role.Equals("Programme Coordinator"))
                    {
                        await _userManager.AddToRoleAsync(newUser, "Approver");
                        await _userManager.AddToRoleAsync(newUser, "Programme Coordinator");
                    }

                    // add courses for approvers
                    if (model.Role.Equals("Academic Manager") || model.Role.Equals("Programme Coordinator"))
                    {
                        foreach (var id in courseIds)
                        {
                            var approverCourse = new ApproverCourse
                            {
                                ApproverID = newUser.Id,
                                CourseID = id
                            };

                            var courseRes = await _dbContext.ApproverCourses.AddAsync(approverCourse);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Email", "Email provided is already in use.");

                model.CourseModules = await _dbContext.CourseModules
                    .Select(cm => new CourseModuleWithCodes
                    {
                        CourseModuleId = cm.CourseModuleID,
                        CourseCode = cm.Course.CourseCode,
                        ModuleId = cm.Module.ModuleID,
                        ModuleCode = cm.Module.ModuleCode,
                    })
                    .ToListAsync();

                var courses = await _dbContext.Courses
                    .Select(c => new { c.CourseID, c.CourseCode })
                    .ToListAsync();

                model.Courses = courses.Select(c => (c.CourseID, c.CourseCode)).ToList();

                return View(model);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //=============================================================================================================================//
        /// <summary>
        /// Load view for updating a user
        /// </summary>
        /// <param name="userId">ID of user to update</param>
        public async Task<IActionResult> Update(string userId)
        {
            // retrive user data
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            // populate view model
            var viewModel = new UpdateUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserId = user.Id,
            };

            // role-based view model population
            if (userRoles.Contains("Lecturer"))
            {
                viewModel.Role = "Lecturer";
                viewModel.HourlyRate = user.HourlyRate;

                var bankAccount = await _dbContext.BankAccounts.Where(b => b.LecturerID.Equals(user.Id)).FirstAsync();
                viewModel.BankAccountId = bankAccount.BankAccountID;
                viewModel.BankName = bankAccount.BankName;
                viewModel.BranchCode = bankAccount.BranchCode;
                viewModel.AccountHolder = bankAccount.AccountHolder;
                viewModel.AccountNumber = bankAccount.AccountNumber;

                var lecturerModuleWithModuleCode = await _dbContext.LecturerModules
                    .Where(lm => lm.LecturerID.Equals(user.Id))
                    .Join(_dbContext.CourseModules,
                        lm => lm.CourseModuleID,
                        cm => cm.CourseModuleID,
                        (lm, cm) => new { LecturerModule = lm, CourseModule = cm })
                    .Join(_dbContext.Modules,
                        lmcm => lmcm.CourseModule.ModuleID,
                        m => m.ModuleID,
                        (lmcm, m) => new { LecturerModule = lmcm.LecturerModule, CourseModule = lmcm.CourseModule, Module = m })
                    .Select(lmcmm => new { LecturerModuleId = lmcmm.LecturerModule.LecturerModuleID, ModuleCode = lmcmm.Module.ModuleCode, CourseModuleId = lmcmm.CourseModule.CourseModuleID })
                    .ToListAsync();

                List<CourseModuleWithCodes> courseModules = await _dbContext.CourseModules
                    .Join(_dbContext.Modules,
                        cm => cm.ModuleID,
                        m => m.ModuleID,
                        (cm, m) => new { CourseModule = cm, Module = m })
                    .Join(_dbContext.Courses,
                        cmm => cmm.CourseModule.CourseID,
                        c => c.CourseID,
                        (cmm, c) => new CourseModuleWithCodes
                        {
                            CourseModuleId = cmm.CourseModule.CourseModuleID,
                            CourseId = c.CourseID,
                            ModuleId = cmm.Module.ModuleID,
                            CourseCode = c.CourseCode,
                            ModuleCode = cmm.Module.ModuleCode
                        }).ToListAsync();

                viewModel.CourseModules = courseModules;
                viewModel.LecturerModules = lecturerModuleWithModuleCode.Select(lmmc => (lmmc.CourseModuleId, lmmc.ModuleCode)).ToList();
                viewModel.LecturerCourseModulesJson = "";
                viewModel.AllModules = await _dbContext.Modules.Select(m => m.ModuleCode).ToListAsync();
            }
            else if (userRoles.Contains("Approver"))
            {
                viewModel.Role = userRoles.Contains("Academic Manager") ? "Academic Manager" : "Programme Coordinator";
                var courses = await _dbContext.Courses
                    .Join(_dbContext.ApproverCourses,
                        c => c.CourseID,
                        ac => ac.CourseID,
                        (c, ac) => new { CourseId = c.CourseID, CourseCode = c.CourseCode, ApproverId = ac.ApproverID })
                    .ToListAsync();

                viewModel.Courses = courses.Select(ac => (ac.CourseId, ac.CourseCode)).Distinct().ToList();
                viewModel.ApproverCourses = courses.Where(ac => ac.ApproverId.Equals(user.Id)).Select(ac => (ac.CourseId, ac.CourseCode)).ToList();
                viewModel.CoursesJson = "";
                viewModel.AllCourses = await _dbContext.Courses.Select(c => c.CourseCode).ToListAsync();
            }

            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="model">Update view model</param>
        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            var lecturerCourseModuleIds = JsonConvert.DeserializeObject<List<int>>(model.LecturerCourseModulesJson);
            var approverCourseIds = JsonConvert.DeserializeObject<List<int>>(model.CoursesJson);

            // validation
            if (!string.IsNullOrEmpty(model.Role) && model.Role.Equals("Lecturer"))
            {
                if (model.HourlyRate <= 0)
                {
                    ModelState.AddModelError("HourlyRate", "Hourly Rate must be greater than zero for Lecturers.");
                }

                if (lecturerCourseModuleIds.Count < 1)
                {
                    ModelState.AddModelError("LecturerCourseModulesJson", "At least one module must be assigned to Lecturer.");
                }

                if (string.IsNullOrEmpty(model.BankName))
                {
                    ModelState.AddModelError("BankName", "Bank name is required");
                }

                if (string.IsNullOrEmpty(model.BranchCode))
                {
                    ModelState.AddModelError("BranchCode", "Branch code is required");
                }

                if (string.IsNullOrEmpty(model.AccountNumber))
                {
                    ModelState.AddModelError("AccountNumber", "Bank account number is required");
                }

                if (string.IsNullOrEmpty(model.AccountHolder))
                {
                    ModelState.AddModelError("AccountHolder", "Bank account holder name is required.");
                }
            }

            if (!string.IsNullOrEmpty(model.Role) && (model.Role.Equals("Academic Manager") || model.Role.Equals("Programme Coordinator")) && (approverCourseIds != null && approverCourseIds.Count < 1))
            {
                ModelState.AddModelError("CoursesJson", $"At least on course must be assigned to {(model.Role.Equals("Academic Manager") ? "an academic manager." : "a programme manager.")}");
            }

            if (!ModelState.IsValid)
            {
                List<CourseModuleWithCodes> courseModules = await _dbContext.CourseModules
                    .Join(_dbContext.Modules,
                        cm => cm.ModuleID,
                        m => m.ModuleID,
                        (cm, m) => new { CourseModule = cm, Module = m })
                    .Join(_dbContext.Courses,
                        cmm => cmm.CourseModule.CourseID,
                        c => c.CourseID,
                        (cmm, c) => new CourseModuleWithCodes
                        {
                            CourseModuleId = cmm.CourseModule.CourseModuleID,
                            CourseId = c.CourseID,
                            ModuleId = cmm.Module.ModuleID,
                            CourseCode = c.CourseCode,
                            ModuleCode = cmm.Module.ModuleCode
                        }).ToListAsync();

                model.CourseModules = courseModules;
                var approverCourses = await _dbContext.ApproverCourses
                   .Join(_dbContext.Courses,
                       ac => ac.CourseID,
                       c => c.CourseID,
                       (ac, c) => new { ApproverCourse = ac, CourseCode = c.CourseCode })
                   .Select(acc => new { acc.ApproverCourse.CourseID, acc.CourseCode })
                   .ToListAsync();

                model.Courses = approverCourses.Select(ac => (ac.CourseID, ac.CourseCode)).ToList();
                model.AllModules = await _dbContext.Modules.Select(m => m.ModuleCode).ToListAsync();
                model.AllCourses = await _dbContext.Courses.Select(c => c.CourseCode).ToListAsync();

                return View(model);
            }

            // update user
            var user = await _userManager.FindByIdAsync(model.UserId);
            // reset password if provided (Microsoft docs)
            if (!string.IsNullOrEmpty(model.Password))
            {
                // reset using user manager
                var pwResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, pwResetToken, model.Password);
            }

            // update name and email
            user.UserName = model.FirstName + model.LastName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            if (model.Role.Equals("Lecturer"))
            {
                user.HourlyRate = model.HourlyRate;

                // update bank account
                var bankAccount = await _dbContext.BankAccounts.Where(b => b.LecturerID.Equals(model.UserId)).FirstAsync();
                bankAccount.BankAccountID = model.BankAccountId;
                bankAccount.BankName = model.BankName;
                bankAccount.BranchCode = model.BranchCode;
                bankAccount.AccountHolder = model.AccountHolder;
                bankAccount.AccountNumber = model.AccountNumber;

                // update modules
                // disassociate course modules from lecturer
                var currentLecturerCourseModuleIds = await _dbContext.LecturerModules.Where(lm => lm.LecturerID.Equals(model.UserId))
                    .Select(lm => lm.CourseModuleID).ToListAsync();
                var modulesToRemove = await _dbContext.LecturerModules.Where(lm => lm.LecturerID.Equals(model.UserId) && !lecturerCourseModuleIds.Contains(lm.CourseModuleID)).ToListAsync();

                if (modulesToRemove != null && modulesToRemove.Count() > 0)
                {
                    _dbContext.LecturerModules.RemoveRange(modulesToRemove);
                }

                //associate new course modules with lecturer
                var modulesToAdd = await _dbContext.CourseModules.Where(cm => lecturerCourseModuleIds.Contains(cm.CourseModuleID) && !currentLecturerCourseModuleIds.Contains(cm.CourseModuleID))
                    .Select(cm => new LecturerModule { LecturerID = model.UserId, CourseModuleID = cm.CourseModuleID })
                    .ToListAsync();

                if (modulesToAdd != null && modulesToAdd.Count > 0)
                {
                    _dbContext.LecturerModules.AddRange(modulesToAdd);
                }

            }
            else if (model.Role.Equals("Academic Manager") || model.Role.Equals("Programme Coordinator"))
            {
                // update course authroisations
                var currentCourseIds = await _dbContext.ApproverCourses.Where(ac => ac.ApproverID.Equals(model.UserId))
                    .Select(ac => ac.CourseID)
                    .ToListAsync();

                // disassociate courses from user
                var coursesToRemove = await _dbContext.ApproverCourses.Where(ac => ac.ApproverID.Equals(model.UserId) && !approverCourseIds.Contains(ac.CourseID)).ToListAsync();

                if (coursesToRemove != null && coursesToRemove.Count > 0)
                {
                    _dbContext.ApproverCourses.RemoveRange(coursesToRemove);
                }

                // associate new courses with user
                var coursesToAdd = await _dbContext.Courses.Where(c => approverCourseIds.Contains(c.CourseID) && !currentCourseIds.Contains(c.CourseID))
                    .Select(ac => new ApproverCourse
                    {
                        ApproverID = model.UserId,
                        CourseID = ac.CourseID
                    })
                    .ToListAsync();

                if (coursesToAdd != null && coursesToAdd.Count > 0)
                {
                    _dbContext.ApproverCourses.AddRange(coursesToAdd);
                }

            }
            // save updates
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        //=============================================================================================================================//
        /// <summary>
        /// Soft deletes a user (removes personal information and login)
        /// </summary>
        /// <param name="UserId">ID of the user to delete</param>
        public async Task<IActionResult> Delete(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null)
            {
                user.IsDeleted = 1;
                user.FirstName = "Deleted";
                user.LastName = "Deleted";
                user.UserName = "Deleted";
                user.Email = "Deleted";
                user.PasswordHash = "Deleted";
                if (await _userManager.IsInRoleAsync(user, "Lecturer"))
                {
                    var bankAccount = await _dbContext.BankAccounts.Where(b => b.LecturerID.Equals(user.Id)).FirstAsync();
                    _dbContext.Remove(bankAccount);
                }

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\