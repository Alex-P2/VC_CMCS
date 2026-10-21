// References
// https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class Claim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimID { get; set; }
        [Required]
        public string LecturerID { get; set; }
        [Required]
        public int LecturerModuleID { get; set; }
        [Required]
        public double HoursWorked { get; set; }
        [Required]
        public int HourlyRate { get; set; }
        [Required]
        public int StatusID { get; set; }
        public string AdditionalNotes { get; set; } = string.Empty;
        public int? InvoiceID { get; set; }
        public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;

        [ForeignKey(nameof(LecturerID))]
        public ApplicationUser Lecturer { get; set; }

        [ForeignKey(nameof(StatusID))]
        public ClaimStatus Status { get; set; }

        [ForeignKey(nameof(InvoiceID))]
        public Invoice Invoice { get; set; }

        [ForeignKey(nameof(LecturerModuleID))]
        public LecturerModule LecturerModule { get; set; }

        public ICollection<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
        public ICollection<ClaimApproval> ClaimApprovals { get; set; } = new List<ClaimApproval>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\