using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class ClaimInvoiceDetail
    {
        public int InvoiceNumber { get; set; }
        public string InvoiceeName { get; set; }
        public string InvoiceeEmail { get; set; }
        public BankAccount BankAccount { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public double TotalAmount { get; set; }

        public string Issuer { get; set; } = "Contract Lecturer Monthly Claims System";
        public string IssuerEmail { get; set; } = "billing@cmcs.co.za";

        public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly PaymentDue { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(14));

    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\