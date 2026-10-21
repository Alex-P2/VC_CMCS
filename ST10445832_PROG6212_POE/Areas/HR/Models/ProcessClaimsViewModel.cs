
namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class ProcessClaimsViewModel
    {
        public List<ProcessClaimsDetail> Claims { get; set; } = [];
        public string Query { get; set; } = string.Empty;
        public bool Filtered { get; set; }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\