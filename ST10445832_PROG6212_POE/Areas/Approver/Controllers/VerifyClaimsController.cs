// References
// https://www.csharptutorial.net/csharp-linq/linq-inner-join/

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
    [Authorize(Roles = "Programme Coordinator")]
    public class VerifyClaimsController : Controller
    {

        /// <summary>
        /// Provides access to Approver role claim management methods
        /// </summary>
        private readonly IApproverRepository _repository;

        /// <summary>
        /// SignalR status event listener
        /// </summary>
        private readonly StatusListener _listener;


        //=============================================================================================================================//
        /// <summary>
        /// Defaut constructor
        /// </summary>
        public VerifyClaimsController(IApproverRepository repository, StatusListener listener)
        {
            _repository = repository;
            _listener = listener;
        }


        //=============================================================================================================================//
        /// <summary>
        /// Loads filtered (optional) non-approved claims
        /// </summary>
        /// <param name="query">Search query to filter claims</param>
        public async Task<IActionResult> Index(string query)
        {
            // get all claims which aren't approved
            List<VerifyClaimRecord> claimRecords = await _repository.GetNonApprovedClaimsAsync();

            // filter if required
            var filter = !string.IsNullOrEmpty(query);
            var statusFilters = new string[] { "Pending", "Verified", "Rejected" };
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
            claimRecords = claimRecords.OrderBy(claimRecord => claimRecord.DateCreated.GetValueOrDefault()).Reverse().ToList();

            VerifyClaimsViewModel viewModel = new VerifyClaimsViewModel
            {
                Claims = claimRecords,
                Filter = statusFilter,
                Filtered = filter,
                Query = query
            };

            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a json object containing the full detail of a claim
        /// </summary>
        /// <param name="claimID">ID of the claim detail to return</param>
        public async Task<JsonResult> GetClaimDetail(int claimID)
        {
            VerifyClaimRecord claimRecord = await _repository.GetVerifyClaimRecordAsync(claimID);

            return Json(claimRecord);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Returns a specific file to be downloaded
        /// </summary>
        /// <param name="documentID">ID of the document to download</param>
        [HttpGet]
        public async Task<FileContentResult> DownloadDocument(int documentID)
        {
            SupportingDocument document = await _repository.GetDocumentAsync(documentID);
            // decrypt file content
            var decryptedContent = CryptographyService.DecryptFile(document.EncryptedContent, document.IV);
            return File(decryptedContent, document.ContentType, document.Title);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Updates the status of a claim to verified
        /// </summary>
        /// <param name="claimID">ID of the claim to verify</param>
        [HttpGet]
        public async Task<IActionResult> VerifyClaim(int claimID)
        {
            var claim = await _repository.GetClaimAsync(claimID);
            if (claim is not null)
            {
                if (claim.StatusID == 2) // status verified
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim already verified." });
                    return RedirectToAction(nameof(Index));
                }
                if (claim.StatusID == 1 || claim.StatusID == 4) // status pending or rejected
                {
                    // update status
                    claim.StatusID = await _repository.GetStatusIdAsync("Verified");
                    var claimApproval = new ClaimApproval
                    {
                        ApproverID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ClaimID = claim.ClaimID
                    };
                    await _repository.AddClaimApprovalAsync(claimApproval);
                    await _repository.SaveChangesAsync();
                    // send update notification for real-time update
                    await _listener.NotifyOnStatusUpdate(claim.ClaimID, "Verified");
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = "Claim Verified." });
                }
            }
            else
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = "Could not complete verification. Please refresh the page." });
            }
            return RedirectToAction(nameof(Index));
        }


        //=============================================================================================================================//
        /// <summary>
        /// Updates the status of a claim to rejected
        /// </summary>
        /// <param name="claimID">ID of the claim to reject</param>
        [HttpGet]
        public async Task<IActionResult> RejectClaim(int claimID)
        {
            var claim = await _repository.GetClaimAsync(claimID);
            if (claim is not null && claim.StatusID != 3) // status not approved
            {
                // delete existing approval
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
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\