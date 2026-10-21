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
        public string LecturerID { get; set; }
        [Required]
        public string BankName { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string AccountHolder { get; set; }

        [ForeignKey("LecturerID")]
        public ApplicationUser Lecturer { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\