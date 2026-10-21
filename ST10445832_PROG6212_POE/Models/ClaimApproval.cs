using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class ClaimApproval
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimApprovalID { get; set; }

        [Required]
        public int ClaimID { get; set; }

        [Required]
        public string ApproverID { get; set; } = string.Empty;

        public DateTimeOffset DateApproved { get; set; } = DateTimeOffset.Now;

        [ForeignKey("ClaimID")]
        public Claim Claim { get; set; }

        [ForeignKey("ApproverID")]
        public ApplicationUser Approver { get; set; } = new();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\