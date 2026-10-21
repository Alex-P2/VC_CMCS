using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class LecturerModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LecturerModuleID { get; set; }
        [Required]
        public int CourseModuleID { get; set; }
        [Required]
        public string LecturerID { get; set; }

        [ForeignKey("CourseModuleID")]
        public CourseModule CourseModule { get; set; }

        [ForeignKey("LecturerID")]
        public ApplicationUser Lecturer { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\