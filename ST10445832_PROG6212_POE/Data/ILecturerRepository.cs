// References
// https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database

using ST10445832_PROG6212_POE.Areas.HR.Models;
using ST10445832_PROG6212_POE.Areas.Lecturer.Models;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class ClaimWithStatus
    {
        public Claim Claim { get; set; }
        public ClaimStatus ClaimStatus { get; set; }
    }
    public interface ILecturerRepository
    {
        Task<List<CourseModuleWithCodes>> GetLecturerModulesAsync(string lecturerID);

        Task<List<ClaimHistoryRecord>> GetLecturerClaimsAsync(string lecturerID);

        Task<List<SupportingDocument>> GetClaimDocumentsAsync(int claimID);

        Task<ClaimHistoryRecord> GetClaimDetailAsync(int claimID);

        Task<ApplicationUser> GetUserByIdAsync(string userID);

        Task<Claim> CreateClaimObjectAsync(CreateClaimViewModel model, string lecturerID);

        Task<Claim> AddClaimAsync(Claim claim);

        Task AddDocumentAsync(IFormFile document, int claimID);

        Task AddTempDocumentAsync(TempFile document, int claimID);

        Task<SupportingDocument> GetDocumentAsync(int documentID);

        Task<ClaimWithStatus> GetClaimWithStatusAsync(int claimID);

        Task<int> DeleteClaimDocumentsAsync(int claimID);

        Task<int> DeleteClaimAsync(int claimID);

        Task<(double, int)> GetMonthHoursClaimedAndSessions(int courseModuleID, DateOnly date);

        Task SaveChangesAsync();

    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\