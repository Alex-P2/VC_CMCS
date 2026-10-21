using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InvoiceID { get; set; }
        [Required]
        public byte[] Content { get; set; }
        [Required]
        public DateOnly PaymentDue { get; set; }
        public DateOnly DateCreated { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\