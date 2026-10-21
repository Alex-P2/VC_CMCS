using Microsoft.EntityFrameworkCore;
using ST10445832_PROG6212_POE.Areas.Approver.Models;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class ApproverRepository : IApproverRepository
    {
        private readonly AppDbContext _dbContext;
        public ApproverRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<VerifyClaimRecord>> GetNonApprovedClaimsAsync()
        {
            return await _dbContext.Claims.Where(claim => claim.Lecturer.IsDeleted == 0)
                .Select(claim => new VerifyClaimRecord
                {
                    ClaimID = claim.ClaimID,
                    ClaimantName = $"{claim.Lecturer.UserName}",
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    Status = claim.Status.StatusTitle,
                    AdditionalNotes = claim.AdditionalNotes,
                    Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                    {
                        DocumentID = document.DocumentID,
                        ClaimID = document.ClaimID,
                        Title = document.Title,
                        ContentType = document.ContentType,
                        EncryptedContent = document.EncryptedContent,
                        IV = document.IV,
                    }).ToList(),
                    ScheduledSessions = claim.LecturerModule.CourseModule.CountSessions
                })
                .Where(claimRecord => !claimRecord.Status.Equals("Approved") && !claimRecord.Status.Equals("Processed"))
                .ToListAsync();
        }

        public async Task<List<ApproveClaimsRecord>> GetVerifiedClaimsAsync()
        {
            return await _dbContext.Claims.Where(claim => claim.Lecturer.IsDeleted == 0)
                .Select(claim => new ApproveClaimsRecord
                {
                    ClaimID = claim.ClaimID,
                    ClaimantName = $"{claim.Lecturer.UserName}",
                    VerifierID = claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).ApproverID,
                    VerifierName = $"{claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).Approver.FirstName} " +
                                   $"{claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).Approver.LastName}",
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    Status = claim.Status.StatusTitle,
                    AdditionalNotes = claim.AdditionalNotes,
                    Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                    {
                        DocumentID = document.DocumentID,
                        ClaimID = document.ClaimID,
                        Title = document.Title,
                        ContentType = document.ContentType,
                        EncryptedContent = document.EncryptedContent,
                        IV = document.IV,
                    }).ToList(),
                    ScheduledSessions = claim.LecturerModule.CourseModule.CountSessions
                })
                .Where(claimRecord => claimRecord.Status.Equals("Verified") || claimRecord.Status.Equals("Approved"))
                .ToListAsync();
        }

        public async Task<VerifyClaimRecord> GetVerifyClaimRecordAsync(int claimID)
        {
            return await _dbContext.Claims.Where(claim => claim.Lecturer.IsDeleted == 0)
                .Select(claim => new VerifyClaimRecord
                {
                    ClaimID = claim.ClaimID,
                    ClaimantName = $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}",
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    Status = claim.Status.StatusTitle,
                    AdditionalNotes = claim.AdditionalNotes,
                    DateCreated = claim.DateCreated,
                    Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                    {
                        DocumentID = document.DocumentID,
                        ClaimID = document.ClaimID,
                        Title = document.Title,
                        ContentType = document.ContentType,
                        EncryptedContent = document.EncryptedContent,
                        IV = document.IV,
                    }).ToList(),
                    ScheduledSessions = claim.LecturerModule.CourseModule.CountSessions
                })
                .FirstAsync(claim => claim.ClaimID == claimID);
        }

        public async Task<ApproveClaimsRecord> GetApproveClaimRecordAsync(int claimID)
        {
            return await _dbContext.Claims.Where(claim => claim.Lecturer.IsDeleted == 0)
                .Select(claim => new ApproveClaimsRecord
                {
                    ClaimID = claim.ClaimID,
                    ClaimantName = $"{claim.Lecturer.UserName}",
                    VerifierID = claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).ApproverID,
                    VerifierName = $"{claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).Approver.FirstName} " +
                                   $"{claim.ClaimApprovals.First(ca => ca.ClaimID == claim.ClaimID).Approver.LastName}",
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    Status = claim.Status.StatusTitle,
                    AdditionalNotes = claim.AdditionalNotes,
                    DateCreated = claim.DateCreated,
                    Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                    {
                        DocumentID = document.DocumentID,
                        ClaimID = document.ClaimID,
                        Title = document.Title,
                        ContentType = document.ContentType,
                        EncryptedContent = document.EncryptedContent,
                        IV = document.IV,
                    }).ToList(),
                    ScheduledSessions = claim.LecturerModule.CourseModule.CountSessions
                })
                .FirstAsync(claim => claim.ClaimID == claimID);
        }

        public async Task<SupportingDocument> GetDocumentAsync(int documentID)
        {
            return await _dbContext.SupportingDocuments.FirstAsync(doc => doc.DocumentID == documentID);
        }

        public async Task<Claim?> GetClaimAsync(int claimID)
        {
            return await _dbContext.Claims.Where(claim => claim.Lecturer.IsDeleted == 0 && claim.ClaimID == claimID).FirstAsync();
        }

        public async Task<int> GetStatusIdAsync(string statusTitle)
        {
            return (await _dbContext.ClaimStatuses.FirstAsync(status => status.StatusTitle.Equals(statusTitle))).StatusID;
        }

        public async Task AddClaimApprovalAsync(ClaimApproval claimApproval)
        {
            await _dbContext.ClaimApprovals.AddAsync(claimApproval);
        }

        public async Task DeleteClaimApprovalAsync(int claimID, int statusID)
        {
            await _dbContext.ClaimApprovals.Where(ca => ca.ClaimID == claimID && ca.Claim.StatusID == statusID).ExecuteDeleteAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\