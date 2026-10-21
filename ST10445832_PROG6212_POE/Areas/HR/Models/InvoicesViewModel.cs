namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class InvoicesViewModel
    {
        public string? Query { get; set; }
        public bool Filtered { get; set; }
        public List<InvoiceDetail> Invoices { get; set; } = new List<InvoiceDetail>();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\