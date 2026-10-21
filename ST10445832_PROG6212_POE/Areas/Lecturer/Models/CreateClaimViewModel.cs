using Microsoft.AspNetCore.Mvc.ModelBinding;
using ST10445832_PROG6212_POE.Areas.HR.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ST10445832_PROG6212_POE.Areas.Lecturer.Models
{
    public class CreateClaimViewModel
    {
        [Required]
        public string LecturerID { get; set; } = string.Empty;

        public string LecturerName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Module")]
        public int CourseModuleID { get; set; }

        [Required(ErrorMessage = "Hours worked is required, and must be a valid number.")]
        [DisplayName("Hours Worked")]
        public double HoursWorked { get; set; }

        [Required]
        public int HourlyRate { get; set; }

        public string? AdditionalNotes { get; set; }
        public string? StatusMessage { get; set; } = "Pending";

        public List<CourseModuleWithCodes> Modules { get; set; } = new List<CourseModuleWithCodes>();

        [BindNever]
        public List<IFormFile> SupportingDocuments { get; set; } = new List<IFormFile>();

        public List<TempFile> TempFiles { get; set; } = new List<TempFile>();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\