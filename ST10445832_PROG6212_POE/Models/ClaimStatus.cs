using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class ClaimStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatusID { get; set; }
        [Required]
        public string StatusTitle { get; set; }
        [Required]
        public string StatusMessage { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\