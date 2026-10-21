using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class InvoiceDetail
    {
        public string LecturerName { get; set; }
        public string LecturerEmail { get; set; }
        public Invoice Invoice { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\