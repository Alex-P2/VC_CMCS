using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;
using System.Threading.Tasks;

namespace ST10445832_PROG6212_POE.Areas.HR.Data
{
    public class ReportData
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportData(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //==========================================================================//
        // Returns lecturer statistics
        // Timeframe (months) is optional
        public async Task<LecturerStatistics> LecturerStatisticsData(int? timeframe = null)
        {
            var lecturers = await _userManager.GetUsersInRoleAsync("Lecturer");

            var claimsQueryable = _dbContext.Claims.AsQueryable();
            // limit to timeframe if provided
            if (timeframe.HasValue)
            {
                var cutoffDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-timeframe.Value));
                claimsQueryable = claimsQueryable
                    .Where(claim => DateOnly.FromDateTime(claim.DateCreated.Date) >= cutoffDate);
            }

            // Average hourly rate of all lecturers
            var averageHourlyRate = lecturers.Count == 0 ? 0.0 : await _dbContext.Users
                .Where(user => lecturers.Contains(user))
                .AverageAsync(user => user.HourlyRate);

            // average number of claims submitted per lecturer
            var averageClaims = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .GroupBy(claim => claim.LecturerID)
                .Select(group => group.Count())
                .AverageAsync();

            // average value per claim
            var averageClaimValue = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .AverageAsync(claim => claim.HourlyRate * claim.HoursWorked);

            // average number of hours claimed per lecturer
            var averageHoursClaimed = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .GroupBy(claim => claim.LecturerID)
                .Select(group => group.Sum(claim => claim.HoursWorked))
                .AverageAsync();

            // total claim value
            var totalClaimValue = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable.SumAsync(claim => claim.HourlyRate * claim.HoursWorked);

            return new LecturerStatistics
            {
                AverageHourlyRate = averageHourlyRate,
                AverageClaimsSubmitted = averageClaims,
                AverageHoursClaimed = averageHoursClaimed,
                AverageClaimValue = averageClaimValue,
                TotalClaimValue = totalClaimValue
            };
        }

        public async Task<ApproverStatistics> ApproverStatisticsData(int? timeframe = null)
        {
            var claimsApprovalsQueryable = _dbContext.ClaimApprovals.AsQueryable();
            var claimsQueryable = _dbContext.Claims.AsQueryable();

            // limit to timeframe if provided
            if (timeframe.HasValue)
            {
                var cutoffDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-timeframe.Value));
                Console.WriteLine(cutoffDate);
                claimsApprovalsQueryable = claimsApprovalsQueryable
                    .Where(approval => DateOnly.FromDateTime(approval.DateApproved.Date) >= cutoffDate);

                claimsQueryable = claimsQueryable
                    .Where(claim => DateOnly.FromDateTime(claim.DateCreated.Date) >= cutoffDate);
            }

            // average number of claims processed per approver
            var averageClaimsProcessed = await claimsApprovalsQueryable.CountAsync() == 0 ? 0.0 : await claimsApprovalsQueryable
                .GroupBy(approval => approval.ApproverID)
                .Select(group => group.Count())
                .AverageAsync();

            // rejection rate
            var countClaims = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .CountAsync();
            var countRejected = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .Include(claim => claim.Status)
                .Where(claim => claim.Status.StatusTitle.Equals("Rejected"))
                .CountAsync();

            var claimRejectionRate = (double)countRejected / (double)countClaims * 100;

            // average time from claim to invoice
            var averageTimeToInvoice = await claimsQueryable.CountAsync() == 0 ? 0.0 : await claimsQueryable
                .Where(claim => claim.InvoiceID != null)
                .Select(claim => EF.Functions.DateDiffDay(DateOnly.FromDateTime(claim.DateCreated.Date), claim.Invoice.DateCreated))
                .AverageAsync();

            return new ApproverStatistics
            {
                AverageClaimsProcessed = averageClaimsProcessed,
                ClaimRejectionRate = claimRejectionRate,
                AverageTimeToInvoice = averageTimeToInvoice
            };
        }

        public async Task<HrStatistics> HrStatisticsData(int? timeframe = null)
        {
            var invoicesQueryable = _dbContext.Invoices.AsQueryable();

            if (timeframe.HasValue)
            {
                var cutoffDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-timeframe.Value));

                invoicesQueryable = invoicesQueryable
                    .Where(invoice => invoice.DateCreated >= cutoffDate);
            }

            // total invoices created
            var invoicesCreated = await invoicesQueryable.CountAsync() == 0 ? 0 : await invoicesQueryable.CountAsync();

            // total invoice value
            var totalInvoiceValue = await invoicesQueryable.CountAsync() == 0 ? 0.0 : await invoicesQueryable
                .Include(invoice => invoice.Claims)
                .SumAsync(invoice => invoice.Claims.Sum(claim => claim.HourlyRate * claim.HoursWorked));

            return new HrStatistics
            {
                TotalInvoicesCreated = invoicesCreated,
                TotalInvoiceValue = totalInvoiceValue
            };
        }
    }

    public class LecturerStatistics
    {
        public double AverageHourlyRate { get; set; }
        public double AverageClaimsSubmitted { get; set; }
        public double AverageHoursClaimed { get; set; }
        public double AverageClaimValue { get; set; }
        public double TotalClaimValue { get; set; }
    }

    public class ApproverStatistics
    {
        public double AverageClaimsProcessed { get; set; }
        public double ClaimRejectionRate { get; set; }
        public double AverageTimeToInvoice { get; set; }
    }

    public class HrStatistics
    {
        public int TotalInvoicesCreated { get; set; }
        public double TotalInvoiceValue { get; set; }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\