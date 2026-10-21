using ST10445832_PROG6212_POE.Areas.Approver.Models;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public interface IApproverRepository
    {
        Task<List<VerifyClaimRecord>> GetNonApprovedClaimsAsync();

        Task<List<ApproveClaimsRecord>> GetVerifiedClaimsAsync();

        Task<VerifyClaimRecord> GetVerifyClaimRecordAsync(int claimID);

        Task<ApproveClaimsRecord> GetApproveClaimRecordAsync(int claimID);

        Task<SupportingDocument> GetDocumentAsync(int documentID);

        Task<Claim?> GetClaimAsync(int claimID);

        Task<int> GetStatusIdAsync(string statusTitle);

        Task DeleteClaimApprovalAsync(int claimID, int statusID);

        Task AddClaimApprovalAsync(ClaimApproval claimApproval);

        Task SaveChangesAsync();
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\