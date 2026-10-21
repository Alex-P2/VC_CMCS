using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class CourseModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseModuleID { get; set; }
        [Required]
        public int ModuleID { get; set; }
        [Required]
        public int CourseID { get; set; }
        [Required]
        public int CountSessions { get; set; }

        [ForeignKey("ModuleID")]
        public Module Module { get; set; }

        [ForeignKey("CourseID")]
        public Course Course { get; set; }

        public ICollection<LecturerModule> LecturerModules { get; set; } = new List<LecturerModule>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\