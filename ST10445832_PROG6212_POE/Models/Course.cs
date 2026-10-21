using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }
        [Required]
        public string CourseCode { get; set; }
        [Required]
        public string CourseName { get; set; }
        [Required]
        public string QualificationType { get; set; }

        public ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();
        public ICollection<ApproverCourse> ApproverCourses { get; set; } = new List<ApproverCourse>();

    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\