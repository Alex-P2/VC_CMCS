using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST10445832_PROG6212_POE.Areas.Approver.Models;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;
using System.Security.Claims;

namespace ST10445832_PROG6212_POE.Areas.Approver.Controllers
{
    [Area("Approver")]
    [Authorize(Roles = "Academic Manager")]
    public class ApproveClaimsController : Controller
    {
        private readonly IApproverRepository _repository;
        private readonly StatusListener _listener;
        public ApproveClaimsController(IApproverRepository repository, StatusListener listener)
        {
            _repository = repository;
            _listener = listener;
        }

        public async Task<IActionResult> Index(string query)
        {
            // get verified claims and claims which were approved or rejected by the current user
            List<ApproveClaimsRecord> claimRecords = await _repository.GetVerifiedClaimsAsync();

            // filter if required
            var filter = !string.IsNullOrEmpty(query);
            var statusFilters = new string[] { "Verified", "Approved", "Rejected" };
            var statusFilter = string.Empty;

            if (filter && statusFilters.Contains(query))
            {
                claimRecords = claimRecords.Where(claim => claim.Status.Equals(query)).ToList();
                statusFilter = query;
            }
            else if (filter)
            {
                claimRecords = claimRecords.Where(claim => JsonConvert.SerializeObject(claim).ToLower().Contains(query.ToLower())).ToList();
            }

            // ensure most recent claims are first
            claimRecords = claimRecords.OrderBy(claimRecord => claimRecord.DateCreated.GetValueOrDefault()).ToList();

            ApproveClaimsViewModel viewModel = new ApproveClaimsViewModel
            {
                Claims = claimRecords,
                Filter = statusFilter,
                Filtered = filter,
                Query = query
            };

            return View(viewModel);
        }

        //===================================================================//
        // Returns a json object containing the full detail of a claim
        public async Task<JsonResult> GetClaimDetail(int claimID)
        {
            ApproveClaimsRecord claimRecord = await _repository.GetApproveClaimRecordAsync(claimID);

            return Json(claimRecord);
        }

        //============================================//
        // Returns a specific file to be downloaded
        [HttpGet]
        public async Task<FileContentResult> DownloadDocument(int documentID)
        {
            SupportingDocument document = await _repository.GetDocumentAsync(documentID);
            // decrypt file
            var decryptedContent = CryptographyService.DecryptFile(document.EncryptedContent, document.IV);
            return File(decryptedContent, document.ContentType, document.Title);
        }

        //=============================================//
        // Updated the status of a claim to approved
        [HttpGet]
        public async Task<IActionResult> ApproveClaim(int claimID)
        {
            var claim = await _repository.GetClaimAsync(claimID);
            if (claim is not null)
            {
                if (claim.StatusID == 3) // status approved
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim already approved." });
                    return RedirectToAction(nameof(Index));
                }
                if (claim.StatusID == 2 || claim.StatusID == 4) // status verified or rejected
                {
                    // update status
                    claim.StatusID = await _repository.GetStatusIdAsync("Approved");
                    var claimApproval = new ClaimApproval
                    {
                        ApproverID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ClaimID = claim.ClaimID
                    };
                    await _repository.AddClaimApprovalAsync(claimApproval);
                    await _repository.SaveChangesAsync();
                    // send update notification for real-time update
                    await _listener.NotifyOnStatusUpdate(claim.ClaimID, "Approved");
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim Approved." });
                }
            }
            else
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = "Could not complete approval. Please refresh the page." });
            }
            return RedirectToAction(nameof(Index));
        }

        //=============================================//
        // Updated the status of a claim to rejected
        [HttpGet]
        public async Task<IActionResult> RejectClaim(int claimID)
        {
            var claim = await _repository.GetClaimAsync(claimID);
            if (claim is not null)
            {
                if (claim.StatusID == 4) // status rejected
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim already rejected." });
                    return RedirectToAction(nameof(Index));
                }
                // remove any existing approval
                await _repository.DeleteClaimApprovalAsync(claim.ClaimID, claim.StatusID);
                await _repository.SaveChangesAsync();
                claim.StatusID = await _repository.GetStatusIdAsync("Rejected");
                await _repository.SaveChangesAsync();
                // send update notification for real-time update
                await _listener.NotifyOnStatusUpdate(claim.ClaimID, "Rejected");
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim Rejected." });
            }
            else
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = "Could not complete claim rejection." });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\