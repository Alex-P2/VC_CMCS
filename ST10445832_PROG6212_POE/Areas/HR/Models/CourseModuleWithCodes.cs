namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class CourseModuleWithCodes
    {
        public int CourseModuleId { get; set; }
        public int CourseId { get; set; }
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\