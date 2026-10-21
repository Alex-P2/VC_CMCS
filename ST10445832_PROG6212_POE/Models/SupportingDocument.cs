// References
// https://putridparrot.com/blog/storing-binary-data-using-entity-framework/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class SupportingDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public byte[] EncryptedContent { get; set; }
        [Required]
        public byte[] IV { get; set; }
        [Required]
        public string ContentType { get; set; }
        [Required]
        public int ClaimID { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;

        [ForeignKey("ClaimID")]
        public Claim Claim { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\