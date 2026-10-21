// References
// https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database

using Microsoft.EntityFrameworkCore;
using ST10445832_PROG6212_POE.Areas.HR.Models;
using ST10445832_PROG6212_POE.Areas.Lecturer.Models;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Data
{
    public class LecturerRepository : ILecturerRepository
    {
        /// <summary>
        /// Application database context
        /// </summary>
        private readonly AppDbContext _dbContext;


        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public LecturerRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a list of course modules taught by the specified lecturer
        /// </summary>
        public async Task<List<CourseModuleWithCodes>> GetLecturerModulesAsync(string lecturerID)
        {
            return await _dbContext.LecturerModules
                .Where(lm => lm.LecturerID.Equals(lecturerID))
                .Select(lm => new CourseModuleWithCodes
                {
                    CourseModuleId = lm.CourseModuleID,
                    ModuleCode = lm.CourseModule.Module.ModuleCode,
                    CourseCode = lm.CourseModule.Course.CourseCode,
                    ModuleId = lm.CourseModule.ModuleID,
                    CourseId = lm.CourseModule.CourseID
                })
                .ToListAsync();
        }


        //=============================================================================================================================//
        /// <summary>
        /// returns a list of detailed claims for the specified lecturer
        /// </summary>
        public async Task<List<ClaimHistoryRecord>> GetLecturerClaimsAsync(string lecturerID)
        {
            return await _dbContext.Claims
                .Where(claim => claim.LecturerID.Equals(lecturerID))
                .Select(claim => new ClaimHistoryRecord
                {
                    ClaimID = claim.ClaimID,
                    ClaimantName = $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}",
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    CourseModuleID = claim.LecturerModule.CourseModule.ModuleID,
                    HoursWorked = claim.HoursWorked,
                    HourlyRate = claim.HourlyRate,
                    StatusTitle = claim.Status.StatusTitle,
                    AdditionalNotes = claim.AdditionalNotes,
                    DateCreated = DateOnly.FromDateTime(claim.DateCreated.DateTime),
                    Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                    {
                        DocumentID = document.DocumentID,
                        ClaimID = document.ClaimID,
                        Title = document.Title,
                        ContentType = document.ContentType,
                        EncryptedContent = document.EncryptedContent,
                        IV = document.IV,
                    }).ToList()
                })
                .ToListAsync();
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a list of supporting documents associated with a claim
        /// </summary>
        public async Task<List<SupportingDocument>> GetClaimDocumentsAsync(int claimID)
        {
            return await _dbContext.SupportingDocuments.Where(doc => doc.ClaimID == claimID).ToListAsync();
        }


        //=============================================================================================================================//
        /// <summary>
        /// returns detailed information about a specific claim
        /// </summary>
        public async Task<ClaimHistoryRecord> GetClaimDetailAsync(int claimID)
        {
            return await _dbContext.Claims.Select(claim => new ClaimHistoryRecord
            {
                ClaimID = claim.ClaimID,
                ClaimantName = $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}",
                ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                CourseModuleID = claim.LecturerModule.CourseModule.ModuleID,
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                StatusTitle = claim.Status.StatusTitle,
                AdditionalNotes = claim.AdditionalNotes,
                CountApprovals = claim.ClaimApprovals.Count(),
                DateCreated = DateOnly.FromDateTime(claim.DateCreated.DateTime),
                Documents = claim.SupportingDocuments.Select(document => new SupportingDocument
                {
                    DocumentID = document.DocumentID,
                    ClaimID = document.ClaimID,
                    Title = document.Title,
                    ContentType = document.ContentType,
                    EncryptedContent = document.EncryptedContent,
                    IV = document.IV,
                }).ToList()
            })
            .FirstAsync(claim => claim.ClaimID == claimID);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a user object using the user id
        /// </summary>
        public async Task<ApplicationUser> GetUserByIdAsync(string userID)
        {
            return await _dbContext.Users.FirstAsync(user => user.Id.Equals(userID));
        }


        //=============================================================================================================================//
        /// <summary>
        /// Creates a claim object
        /// </summary>
        public async Task<Models.Claim> CreateClaimObjectAsync(CreateClaimViewModel model, string lecturerID)
        {
            return new Models.Claim
            {
                LecturerID = lecturerID,
                LecturerModuleID = await _dbContext.LecturerModules.Where(lm => lm.CourseModule.CourseModuleID == model.CourseModuleID && lm.LecturerID.Equals(lecturerID)).Select(lm => lm.LecturerModuleID).FirstAsync(),
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                StatusID = (await _dbContext.ClaimStatuses.FirstAsync(status => status.StatusTitle.Equals(model.StatusMessage))).StatusID,
                AdditionalNotes = model.AdditionalNotes ?? string.Empty,
            };
        }


        //=============================================================================================================================//
        /// <summary>
        /// Adds a claim to the database and returns the created claim
        /// </summary>
        public async Task<Models.Claim> AddClaimAsync(Models.Claim claim)
        {
            var res = await _dbContext.Claims.AddAsync(claim);
            return res.Entity;
        }


        //=============================================================================================================================//
        /// <summary>
        /// adds a supporting document to the database from a IFormFile
        /// </summary>
        public async Task AddDocumentAsync(IFormFile document, int claimID)
        {
            Stream stream = document.OpenReadStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            byte[] bytes = binaryReader.ReadBytes((int)stream.Length);
            // encrypt file
            var encryptionResponse = CryptographyService.EncryptFile(bytes);

            var docRes = await _dbContext.SupportingDocuments.AddAsync(new SupportingDocument
            {
                ClaimID = claimID,
                ContentType = document.ContentType,
                EncryptedContent = encryptionResponse.Content,
                IV = encryptionResponse.IV,
                Title = document.FileName
            });
        }


        //=============================================================================================================================//
        /// <summary>
        /// adds a supporting document to the database from a temp file
        /// </summary>
        public async Task AddTempDocumentAsync(TempFile tempFile, int claimID)
        {
            Byte[] bytes = System.IO.File.ReadAllBytes(tempFile.Path);
            // encrypt file
            var encryptionResponse = CryptographyService.EncryptFile(bytes);

            var tempRes = await _dbContext.SupportingDocuments.AddAsync(new SupportingDocument
            {
                ClaimID = claimID,
                ContentType = tempFile.ContentType,
                EncryptedContent = encryptionResponse.Content,
                IV = encryptionResponse.IV,
                Title = tempFile.FileName
            });
        }


        //=============================================================================================================================//
        /// <summary>
        /// returns a specific supporting document
        /// </summary>
        public async Task<SupportingDocument> GetDocumentAsync(int documentID)
        {
            return await _dbContext.SupportingDocuments.FirstAsync(doc => doc.DocumentID == documentID);
        }

        public async Task<ClaimWithStatus> GetClaimWithStatusAsync(int claimID)
        {
            return await _dbContext.Claims
                .Join(_dbContext.ClaimStatuses,
                        c => c.StatusID,
                        cs => cs.StatusID,
                        (c, cs) => new ClaimWithStatus { Claim = c, ClaimStatus = cs })
                .Where(ccs => ccs.Claim.ClaimID == claimID).FirstAsync();
        }

        public async Task<int> DeleteClaimDocumentsAsync(int claimID)
        {
            return await _dbContext.SupportingDocuments.Where(sd => sd.ClaimID == claimID).ExecuteDeleteAsync();
        }

        public async Task<int> DeleteClaimAsync(int claimID)
        {
            return await _dbContext.Claims.Where(c => c.ClaimID == claimID).ExecuteDeleteAsync();
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns the number of hours claimed for a module
        /// </summary>
        public async Task<(double, int)> GetMonthHoursClaimedAndSessions(int courseModuleID, DateOnly date)
        {
            var claimedHours = await _dbContext.Claims
                .Where(claim => claim.LecturerModule.CourseModuleID == courseModuleID
                            && claim.DateCreated.Month.Equals(date.Month)
                            && claim.DateCreated.Year.Equals(date.Year)
                            && !claim.Status.StatusTitle.Equals("Rejected"))
                .SumAsync(claim => claim.HoursWorked);
            var courseModule = await _dbContext.CourseModules.FindAsync(courseModuleID);
            var sessions = courseModule.CountSessions;

            return (claimedHours, sessions);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\