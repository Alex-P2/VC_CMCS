using System.ComponentModel.DataAnnotations;

namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Letters only")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Letters only")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[!@#$%^&*])(?=.*?[0-9]).{8,}$", ErrorMessage = "Must be more than 8 characters and contain at least 1 uppercase letter, 1 lower case letter, 1 symbol, 1 number")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public int HourlyRate { get; set; } = 0; // lecturer only

        public string? BankName { get; set; } = string.Empty;

        public string? BranchCode { get; set; } = string.Empty;

        public string? AccountNumber { get; set; } = string.Empty;

        public string? AccountHolder { get; set; } = string.Empty;

        public List<CourseModuleWithCodes> CourseModules { get; set; } = new List<CourseModuleWithCodes>();
        public List<(int, string)> SelectedCourseModules { get; set; } = new List<(int, string)>(); // modules for lecturer
        public string? ModulesJson { get; set; } = string.Empty; // serialised modules data

        public List<(int, string)> Courses { get; set; } = new List<(int, string)>();
        public List<(int, string)> SelectedCourses { get; set; } = new List<(int, string)>(); // courses for academic manager or programme coordinator
        public string? CoursesJson { get; set; } = string.Empty; // serialised courses data
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\