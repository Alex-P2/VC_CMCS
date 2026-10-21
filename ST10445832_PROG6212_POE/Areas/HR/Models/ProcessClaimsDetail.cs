namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class ProcessClaimsDetail
    {
        public string LecturerName { get; set; } = string.Empty;
        public string LecturerID { get; set; } = string.Empty;
        public int ClaimID { get; set; }
        public string ModuleCode { get; set; } = string.Empty;
        public double HoursWorked { get; set; }
        public double ClaimAmount { get; set; }
        public string ClaimStatusTitle { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\