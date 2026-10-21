using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleID { get; set; }
        [Required]
        public string ModuleCode { get; set; }
        [Required]
        public string ModuleName { get; set; }

        public ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\