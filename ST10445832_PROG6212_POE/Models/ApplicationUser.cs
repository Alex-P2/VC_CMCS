// References
// https://www.learnentityframeworkcore.com/relationships

using Microsoft.AspNetCore.Identity;

namespace ST10445832_PROG6212_POE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int HourlyRate { get; set; } = 0; // for lecturers
        public BankAccount BankAccount { get; set; }
        public ICollection<LecturerModule> LecturerModules { get; set; }
        public ICollection<Claim> SubmittedClaims { get; set; }
        public int IsDeleted { get; set; } = 0; // soft delete value

        public ICollection<ApproverCourse> ApproverCourses { get; set; } = new List<ApproverCourse>();
        public ICollection<ClaimApproval> ClaimApprovals { get; set; } = new List<ClaimApproval>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\