using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.Lecturer.Models
{
    public class ClaimHistoryRecord
    {
        public int ClaimID { get; set; }
        public string ClaimantName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public int CourseModuleID { get; set; }
        public int HourlyRate { get; set; }
        public double HoursWorked { get; set; }
        public string StatusTitle { get; set; } = string.Empty;
        public string AdditionalNotes { get; set; } = string.Empty;
        public int CountApprovals { get; set; }
        public DateOnly DateCreated { get; set; }
        public List<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\