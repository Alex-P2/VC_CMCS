using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class InvoiceDetail
    {
        public string LecturerName { get; set; } = string.Empty;
        public string LecturerEmail { get; set; } = string.Empty;
        public Invoice Invoice { get; set; } = new();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\