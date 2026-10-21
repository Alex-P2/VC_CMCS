namespace ST10445832_PROG6212_POE.Areas.Approver.Models
{
    public class VerifyClaimsViewModel
    {
        public string Filter { get; set; }
        public bool Filtered { get; set; }
        public string Query { get; set; }
        public List<VerifyClaimRecord> Claims { get; set; }
    }
}
