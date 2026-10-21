
// References
// https://www.csharp411.com/read-file-into-byte-array/
// https://improveandrepeat.com/2019/10/working-with-temporary-files-in-net/
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-9.0#upload-large-files-with-streaming
// https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.join?view=net-8.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST10445832_PROG6212_POE.Areas.Lecturer.Models;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;
using System.Security.Claims;

namespace ST10445832_PROG6212_POE.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    [Authorize(Roles = "Lecturer")]
    public class ClaimController : Controller
    {
        /// <summary>
        /// Lecturer data access methods 
        /// </summary>
        private readonly ILecturerRepository _repository;


        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClaimController(ILecturerRepository repository)
        {
            _repository = repository;
        }


        //=============================================================================================================================//
        /// <summary>
        /// Displays lecturer home with table of user's existing claims (filtered)
        /// </summary>
        /// <param name="query">Optional search query for filter</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string query)
        {
            List<ClaimHistoryRecord> claimHistoryRecords = await _repository.GetLecturerClaimsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // filter records if required
            bool filtered = !string.IsNullOrEmpty(query);
            if (filtered)
            {
                claimHistoryRecords = claimHistoryRecords.Where(claim => JsonConvert.SerializeObject(claim).ToLower().Contains(query.ToLower())).ToList();
            }
            claimHistoryRecords = claimHistoryRecords.OrderBy(claim => claim.StatusTitle).ToList();
            var viewModel = new ClaimHistoryViewModel
            {
                Claims = claimHistoryRecords,
                Filtered = filtered,
                Query = query
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateClaimViewModel();
            var lecturerID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecturer = await _repository.GetUserByIdAsync(lecturerID);
            if (!string.IsNullOrEmpty(lecturerID))
            {
                viewModel.LecturerID = lecturerID;
                var lecturerModules = await _repository.GetLecturerModulesAsync(lecturerID);
                viewModel.Modules = lecturerModules;
                viewModel.HourlyRate = lecturer.HourlyRate;
                viewModel.LecturerName = $"{lecturer.FirstName} {lecturer.LastName}";
                return View(viewModel);
            }
            TempData["ToastData"] = "There was an error in authenticating you.";
            return RedirectToAction("Index", "Home");
        }


        //=============================================================================================================================//
        /// <summary>
        /// Performs validation on claim inputs and creates a new claim in the database
        /// </summary>
        /// <param name="model">View model</param>
        [HttpPost]
        [RequestSizeLimit(500_000_000)] // (Microsoft Docs)
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        public async Task<IActionResult> Create(CreateClaimViewModel model)
        {
            try
            {
                // validation of document type
                List<string> permittedFileTypes = new List<string> { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/pdf" };

                // get files (Microsoft Docs)
                var formFeature = Request.HttpContext.Features.GetRequiredFeature<IFormFeature>();
                await formFeature.ReadFormAsync(CancellationToken.None);

                var formFiles = formFeature.Form.Files;

                // validation of document size (< 5MB)
                if (formFiles.Any(file => (file.Length / (1024.0 * 1024.0)) > 5))
                {
                    ModelState.AddModelError("SupportingDocuments", "Files must be smaller than 5MB");
                }

                if (formFiles.Any(file => !permittedFileTypes.Contains(file.ContentType)))
                {
                    ModelState.AddModelError("SupportingDocuments", "pdf, docx, xlsx Only");
                }

                // remove any duplicate or invalid (file type/size) files
                FormFileCollection files = new FormFileCollection();
                files.AddRange(formFiles.Where(file =>
                    !model.TempFiles.Select(tf => tf.FileName).Contains(file.FileName)
                    && permittedFileTypes.Contains(file.ContentType)
                    && ((file.Length / (1024.0 * 1024.0)) < 5))
                    .ToList());

                // validation of module id
                if (!(model.CourseModuleID > 0))
                {
                    ModelState.AddModelError("CourseModuleID", "Please select a module");
                }

                // validation of hours worked
                if (!(model.HoursWorked > 0))
                {
                    ModelState.AddModelError("HoursWorked", "Enter a value greater than zero");
                }

                // validation of hourly rate
                if (!(model.HourlyRate > 0))
                {
                    ModelState.AddModelError("HourlyRate", "Enter a value greater than zero");
                }

                var lecturerID = model.LecturerID;

                if (!ModelState.IsValid)
                {
                    model.Modules = await _repository.GetLecturerModulesAsync(lecturerID);
                    if (files.Count > 0)
                    {   // create temp files for valid documents to avoid re-upload
                        foreach (var file in files)
                        {
                            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            using (var stream = new FileStream(tempPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            ;

                            model.TempFiles.Add(new TempFile { FileName = file.FileName, Path = tempPath, ContentType = file.ContentType });
                        }
                    }

                    return View(model);
                }

                if (model.AdditionalNotes is null)
                {
                    model.AdditionalNotes = "No notes";
                }

                var passesChecks = await PassesAutomatedApproval(model);

                if (passesChecks.Item1 == false)
                {
                    model.StatusMessage = "Pending";
                    model.AdditionalNotes += $"\n\n{passesChecks.Item2}"; // add automation flag to notes
                }
                else if (passesChecks.Item1 == true)
                {
                    model.StatusMessage = "Approved";
                    model.AdditionalNotes += $"\n\n{passesChecks.Item2}";
                }

                // create new claim
                var newClaim = await _repository.CreateClaimObjectAsync(model, lecturerID);
                newClaim = await _repository.AddClaimAsync(newClaim);
                await _repository.SaveChangesAsync();

                //save files
                if (files.Count > 0)
                {
                    foreach (var document in files)
                    {
                        await _repository.AddDocumentAsync(document, newClaim.ClaimID);
                        await _repository.SaveChangesAsync();
                    }
                }

                if (model.TempFiles.Count > 0)
                {
                    foreach (var tempFile in model.TempFiles)
                    {
                        await _repository.AddTempDocumentAsync(tempFile, newClaim.ClaimID);
                        await _repository.SaveChangesAsync();
                        // delete temp files
                        var file = new FileInfo(tempFile.Path);
                        file.Delete();
                    }
                }

                return RedirectToAction("ClaimCreated", "Home", new { success = true, toastMessage = "Claim created successfully!" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Request body too large. The max request body size is 30000000 bytes."))
                {
                    return RedirectToAction("ClaimCreated", "Home", new { success = false, toastMessage = "File size limit is 5MB for supporting documents" });
                }

                return RedirectToAction("ClaimCreated", "Home", new { success = false, ToastMessage = "Failed to create claim. Please try again." });
            }
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a json object containing the full detail of a claim
        /// </summary>
        /// <param name="claimID">ID of claim detail to fetch</param>
        [HttpPost]
        public async Task<JsonResult> GetClaimDetail(int claimID)
        {
            ClaimHistoryRecord claimRecord = await _repository.GetClaimDetailAsync(claimID);

            return Json(claimRecord);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a specific file to be downloaded
        /// </summary>
        [HttpGet]
        public async Task<FileContentResult> DownloadDocument(int documentID)
        {
            SupportingDocument document = await _repository.GetDocumentAsync(documentID);
            // decrypt file
            var decryptedContent = CryptographyService.DecryptFile(document.EncryptedContent, document.IV);
            return File(decryptedContent, document.ContentType, document.Title);
        }


        //=============================================================================================================================//
        /// <summary>
        /// deletes a claim, only if it is still pending approval
        /// </summary>
        public async Task<IActionResult> DeleteClaim(int claimID)
        {
            try
            {
                var claimDetail = await _repository.GetClaimWithStatusAsync(claimID);

                if (claimDetail is not null && claimDetail.ClaimStatus.StatusTitle.Equals("Pending"))
                {
                    // delete documents
                    var rowsDeleted = await _repository.DeleteClaimDocumentsAsync(claimID);
                    // detele claim
                    rowsDeleted += await _repository.DeleteClaimAsync(claimID);
                    if (rowsDeleted > 0)
                    {
                        TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = $"Claim {claimID} deleted, along with associated data" });
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = $"No claim found matching claim ID: {claimID}." });
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = "Only claims with 'Pending' status can be deleted" });
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = $"No claim found matching claim ID: {claimID}." });
                return RedirectToAction(nameof(Index));
            }
        }


        //=============================================================================================================================//
        /// <summary>
        /// Performs automated approval checks
        /// </summary>
        private async Task<(bool, string)> PassesAutomatedApproval(CreateClaimViewModel claim)
        {
            try
            {
                // hourly rate must match lecturer rate
                if (!claim.HourlyRate.Equals((await _repository.GetUserByIdAsync(claim.LecturerID)).HourlyRate))
                {
                    return (false, "Automation Flagged: Hourly rate mismatch.");
                }

                // claimed hours (month total) must not exceed scheduled sessions + 20%
                var date = DateOnly.FromDateTime(DateTime.Now);
                (double, int) hoursClaimedAndSessions = await _repository.GetMonthHoursClaimedAndSessions(claim.CourseModuleID, date);
                if ((hoursClaimedAndSessions.Item1 + claim.HoursWorked) >
                    ((double)hoursClaimedAndSessions.Item2 + ((double)hoursClaimedAndSessions.Item2 * 0.2)))
                {
                    return (false, "Automation Flagged: Claimed hours exceeded scheduled sessions for this month + 20%.");
                }
                // total claimed hours for the month must not exceed 180 hours
                if (hoursClaimedAndSessions.Item1 + claim.HoursWorked > 180)
                {
                    return (false, "Automation Flagged: Monthly claimd hours exceeded limit of 180 hours.");
                }

                return (true, "Automation Passed: Claim approved automatically");
            }
            catch (Exception)
            {
                return (false, "Automation Flagged: System error during automated checks.");
            }
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\