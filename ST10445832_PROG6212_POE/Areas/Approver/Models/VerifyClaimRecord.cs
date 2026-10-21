using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.Approver.Models
{
    public class VerifyClaimRecord
    {
        public int ClaimID { get; set; }
        public string ClaimantName { get; set; }
        public string ModuleCode { get; set; }
        public double HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public string Status { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
        public List<SupportingDocument> Documents { get; set; } = new List<SupportingDocument>();
        public int ScheduledSessions { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\