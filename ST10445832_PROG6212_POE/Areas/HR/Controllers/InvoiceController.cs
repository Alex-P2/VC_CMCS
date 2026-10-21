// References
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.frombodyattribute?view=aspnetcore-9.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using ST10445832_PROG6212_POE.Areas.HR.Data;
using ST10445832_PROG6212_POE.Areas.HR.Models;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HR")]
    public class InvoiceController : Controller
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly AppDbContext _dbContext;


        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvoiceController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //=============================================================================================================================//
        public IActionResult Index()
        {
            return View();
        }


        //=============================================================================================================================//
        /// <summary>
        /// Displays filtered (optional) approved claims which have yet to be invoiced
        /// </summary>
        /// <param name="query">Search query from claim filter</param>
        public async Task<IActionResult> ProcessClaims(string query)
        {
            var claimsDetail = await _dbContext.Claims.Where(claim => claim.Status.StatusTitle.Equals("Approved") && claim.InvoiceID == null)
                .Select(claim => new ProcessClaimsDetail
                {
                    LecturerID = claim.LecturerID,
                    LecturerName = $"{claim.Lecturer.FirstName} {claim.Lecturer.LastName}",
                    ClaimID = claim.ClaimID,
                    ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                    HoursWorked = claim.HoursWorked,
                    ClaimAmount = claim.HoursWorked * claim.HourlyRate,
                    ClaimStatusTitle = claim.Status.StatusTitle,
                    DateCreated = DateOnly.FromDateTime(claim.DateCreated.DateTime)
                })
                .ToListAsync();

            // filter records if required
            bool filtered = !string.IsNullOrEmpty(query);
            if (filtered)
            {
                claimsDetail = claimsDetail.Where(claim => JsonConvert.SerializeObject(claim).ToLower().Contains(query.ToLower())).ToList();
            }

            var viewModel = new ProcessClaimsViewModel
            {
                Claims = claimsDetail,
                Filtered = filtered,
                Query = query
            };

            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Displays invoices for claims
        /// </summary>
        public async Task<IActionResult> Invoices()
        {
            var invoicesWithDetail = await _dbContext.Invoices
                .Include(invoice => invoice.Claims)
                    .ThenInclude(claim => claim.Lecturer)
                .Select(invoice => new InvoiceDetail
                {
                    LecturerName = $"{invoice.Claims.First().Lecturer.FirstName} {invoice.Claims.First().Lecturer.LastName}",
                    LecturerEmail = invoice.Claims.First().Lecturer.Email,
                    Invoice = invoice
                })
                .ToListAsync();

            var viewModel = new InvoicesViewModel
            {
                Invoices = invoicesWithDetail
            };

            return View(viewModel);
        }


        //=============================================================================================================================//
        /// <summary>
        /// Sets the status of a specified claim to rejected
        /// </summary>
        /// <param name="claimID">ID of claim to be rejected</param>
        public async Task<IActionResult> RejectClaim(string claimID)
        {
            var claim = await _dbContext.Claims.FindAsync(int.Parse(claimID));

            if (claim == null)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Claim not found.", ToastType = false });
                return RedirectToAction("Index");
            }

            claim.StatusID = await _dbContext.ClaimStatuses.Where(status => status.StatusTitle.Equals("Rejected"))
                .Select(status => status.StatusID)
                .FirstAsync();
            var changes = await _dbContext.SaveChangesAsync();
            if (changes > 0)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Claim rejected", ToastType = true });
            }
            return RedirectToAction("ProcessClaims");
        }


        //=============================================================================================================================//
        /// <summary>
        /// Generates an invoice for one or more lecturer claims
        /// </summary>
        /// <param name="ClaimIDs">ID of the claim being invoiced</param>
        [HttpPost]
        [Route("HR/Invoice/GenerateInvoice")]
        public async Task<JsonResult> GenerateInvoice([FromBody] List<int> ClaimIDs) // (Microsoft docs)
        {
            try
            {
                // get claim Ids from request
                if (ClaimIDs == null || ClaimIDs.Count == 0)
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "No claim IDs provided for processing", ToastType = false });
                    new JsonResult(new { redirectUrl = Url.Action("ProcessClaims") });
                }
                // get data for invoice
                ClaimInvoiceDetail invoiceDetail = new ClaimInvoiceDetail();

                // invoice number
                var invoices = await _dbContext.Invoices.OrderBy(invoice => invoice.InvoiceID).ToListAsync();
                if (invoices == null || invoices.Count == 0)
                {
                    invoiceDetail.InvoiceNumber = 1;
                }
                else
                {
                    invoiceDetail.InvoiceNumber = invoices.Last().InvoiceID + 1;
                }

                // relevant claim information
                var claims = await _dbContext.Claims.Where(claim => ClaimIDs.Contains(claim.ClaimID))
                    .Include(claim => claim.Lecturer)
                        .ThenInclude(lecturer => lecturer.BankAccount)
                    .Include(claim => claim.LecturerModule.CourseModule.Course)
                    .Include(claim => claim.LecturerModule.CourseModule.Module)
                    .ToListAsync();
                //var claimant = await _userManager.FindByIdAsync(claims.First().LecturerID);
                var claimant = claims.Select(claim => claim.Lecturer).First();

                if (claimant == null || claimant.IsDeleted == 1)
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Lecturer for claim not found.", ToastType = false });
                    return new JsonResult(new { redirectUrl = Url.Action("ProcessClaims") });
                }

                invoiceDetail.InvoiceeName = $"{claimant.FirstName} {claimant.LastName}";
                invoiceDetail.InvoiceeEmail = claimant.Email ?? string.Empty;
                invoiceDetail.BankAccount = claimant.BankAccount;
                invoiceDetail.TotalAmount = claims.Select(claim => claim.HourlyRate * claim.HoursWorked).Sum();

                var invoiceItems = new List<InvoiceItem>();

                foreach (var claim in claims)
                {
                    invoiceItems.Add(new InvoiceItem
                    {
                        CourseCode = claim.LecturerModule.CourseModule.Course.CourseCode,
                        ModuleCode = claim.LecturerModule.CourseModule.Module.ModuleCode,
                        HourlyRate = claim.HourlyRate,
                        HoursWorked = claim.HoursWorked,
                        Total = claim.HourlyRate * claim.HoursWorked
                    });
                }

                invoiceDetail.InvoiceItems = invoiceItems;

                // generate invoice as pdf
                var invoice = new InvoiceDocument(invoiceDetail);
                // save invoice to DB
                byte[] invoiceBytes = invoice.GeneratePdf();
                Invoice newInvoice = new Invoice
                {
                    InvoiceID = invoice.InvoiceDetail.InvoiceNumber,
                    Content = invoiceBytes,
                    PaymentDue = invoice.InvoiceDetail.PaymentDue,
                    DateCreated = invoice.InvoiceDetail.DateCreated
                };

                await _dbContext.Invoices.AddAsync(newInvoice);

                // update claims to reference invoice and set status to processed
                foreach (var claim in claims)
                {
                    claim.InvoiceID = newInvoice.InvoiceID;
                    claim.StatusID = await _dbContext.ClaimStatuses.Where(status => status.StatusTitle.Equals("Processed"))
                        .Select(status => status.StatusID)
                        .FirstAsync();
                }
                _dbContext.UpdateRange(claims);

                await _dbContext.SaveChangesAsync();

                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Invoice generated!", ToastType = true });
                return new JsonResult(new { redirectUrl = Url.Action("ProcessClaims") });
            }
            catch (Exception)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Failed to process claim.", ToastType = false });
                return new JsonResult(new { redirectUrl = Url.Action("ProcessClaims") });
            }
        }


        //=============================================================================================================================//
        /// <summary>
        /// Downloads or opens an invoice as PDF
        /// </summary>
        /// <param name="InvoiceID">ID of the invoice to download or view</param>
        /// <param name="TableAction"></param>
        public async Task<IActionResult> DownloadInvoice(int InvoiceID, string TableAction)
        {
            try
            {
                var invoice = await _dbContext.Invoices.FindAsync(InvoiceID);
                if (invoice == null)
                {
                    TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Failed to invoice not found.", ToastType = false });
                    return RedirectToAction("Invoices");
                }

                var fileBytes = invoice.Content;
                var fileName = $"CMCS_Invoice_{InvoiceID}.pdf";

                if (TableAction.Equals("download"))
                {
                    return File(fileBytes, "application/pdf", fileName);
                }
                else if (TableAction.Equals("view"))
                {
                    return File(fileBytes, "application/pdf");
                }

                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "No action (download/view) specified.", ToastType = false });
                return RedirectToAction("Invoices");

            }
            catch (Exception)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastMessage = "Failed to fetch invoice.", ToastType = false });
                return RedirectToAction("Invoices");
            }
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\