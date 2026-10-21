using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10445832_PROG6212_POE.Models
{
    public class ApproverCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApproverCourseID { get; set; }
        [Required]
        public int CourseID { get; set; }
        [Required]
        public string ApproverID { get; set; }

        [ForeignKey("CourseID")]
        public Course Course { get; set; }

        [ForeignKey("ApproverID")]
        public ApplicationUser Approver { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\