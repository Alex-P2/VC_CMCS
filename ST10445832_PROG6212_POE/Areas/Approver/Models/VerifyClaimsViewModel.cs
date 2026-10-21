namespace ST10445832_PROG6212_POE.Areas.Approver.Models
{
    public class VerifyClaimsViewModel
    {
        public string Filter { get; set; } = string.Empty;
        public bool Filtered { get; set; }
        public string Query { get; set; } = string.Empty;
        public List<VerifyClaimRecord> Claims { get; set; } = [];
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\