using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class BankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankAccountID { get; set; }

        [Required]
        public string LecturerID { get; set; } = string.Empty;

        [Required]
        public string BankName { get; set; } = string.Empty;

        [Required]
        public string BranchCode { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public string AccountHolder { get; set; } = string.Empty;

        [ForeignKey("LecturerID")]
        public ApplicationUser Lecturer { get; set; } = new();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\