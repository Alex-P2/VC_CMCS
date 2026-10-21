// References
// https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
// https://medium.com/@roshanj100/users-and-roles-seeding-in-asp-net-core-identity-with-entity-framework-core-a-step-by-step-guide-28e6f76a18db

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<LecturerModule> LecturerModules { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ApproverCourse> ApproverCourses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ClaimApproval> ClaimApprovals { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ClaimStatus> ClaimStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // define entity relationships
            // claim -> lecturer
            modelBuilder.Entity<Claim>()
                .HasOne(claim => claim.Lecturer)
                .WithMany(lecturer => lecturer.SubmittedClaims)
                .HasForeignKey(claim => claim.LecturerID)
                .OnDelete(DeleteBehavior.Restrict);

            // claim -> lecturer module
            modelBuilder.Entity<Claim>()
                .HasOne(claim => claim.LecturerModule)
                .WithMany(lecturerModule => lecturerModule.Claims)
                .HasForeignKey(claim => claim.LecturerModuleID)
                .OnDelete(DeleteBehavior.Restrict);

            // claim -> status
            modelBuilder.Entity<Claim>()
                .HasOne(claim => claim.Status)
                .WithMany(status => status.Claims)
                .HasForeignKey(claim => claim.StatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // claim -> invoice
            modelBuilder.Entity<Claim>()
                .HasOne(claim => claim.Invoice)
                .WithMany(invoice => invoice.Claims)
                .HasForeignKey(claim => claim.InvoiceID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // supporting document -> claim
            modelBuilder.Entity<SupportingDocument>()
                .HasOne(supportingDocument => supportingDocument.Claim)
                .WithMany(claim => claim.SupportingDocuments)
                .HasForeignKey(supportingDocument => supportingDocument.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            // claim approval -> claim
            modelBuilder.Entity<ClaimApproval>()
                .HasOne(claimApproval => claimApproval.Claim)
                .WithMany(claim => claim.ClaimApprovals)
                .HasForeignKey(claimApproval => claimApproval.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

            // claim approval -> approver
            modelBuilder.Entity<ClaimApproval>()
                .HasOne(claimApproval => claimApproval.Approver)
                .WithMany(approver => approver.ClaimApprovals)
                .HasForeignKey(claimApproval => claimApproval.ApproverID)
                .OnDelete(DeleteBehavior.Restrict);

            // lecturer -> bank account
            modelBuilder.Entity<BankAccount>()
                .HasOne(bankAccount => bankAccount.Lecturer)
                .WithOne(lecturer => lecturer.BankAccount)
                .HasForeignKey<BankAccount>(bankAccount => bankAccount.LecturerID)
                .OnDelete(DeleteBehavior.Cascade);

            // lecturer module -> lecturer
            modelBuilder.Entity<LecturerModule>()
                .HasOne(lecturerModule => lecturerModule.Lecturer)
                .WithMany(lecturer => lecturer.LecturerModules)
                .HasForeignKey(lecturerModule => lecturerModule.LecturerID)
                .OnDelete(DeleteBehavior.Restrict);

            // lecturer module -> course module
            modelBuilder.Entity<LecturerModule>()
                .HasOne(lecturerModule => lecturerModule.CourseModule)
                .WithMany(courseModule => courseModule.LecturerModules)
                .HasForeignKey(lecturerModule => lecturerModule.CourseModuleID)
                .OnDelete(DeleteBehavior.Restrict);

            // course module -> module
            modelBuilder.Entity<CourseModule>()
                .HasOne(courseModule => courseModule.Module)
                .WithMany(module => module.CourseModules)
                .HasForeignKey(courseModule => courseModule.ModuleID)
                .OnDelete(DeleteBehavior.Restrict);

            // course module -> course
            modelBuilder.Entity<CourseModule>()
                .HasOne(courseModule => courseModule.Course)
                .WithMany(course => course.CourseModules)
                .HasForeignKey(courseModule => courseModule.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            // approver course -> course 
            modelBuilder.Entity<ApproverCourse>()
                .HasOne(approverCourse => approverCourse.Course)
                .WithMany(course => course.ApproverCourses)
                .HasForeignKey(approverCourse => approverCourse.CourseID)
                .OnDelete(DeleteBehavior.Cascade);

            // approver course -> approver
            modelBuilder.Entity<ApproverCourse>()
                .HasOne(approverCourse => approverCourse.Approver)
                .WithMany(approver => approver.ApproverCourses)
                .HasForeignKey(approverCourse => approverCourse.ApproverID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\