namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class InvoiceItem
    {
        public string CourseCode { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public int HourlyRate { get; set; }
        public double HoursWorked { get; set; }
        public double Total { get; set; }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\