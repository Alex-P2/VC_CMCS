using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using ST10445832_PROG6212_POE.Areas.HR.Data;
using ST10445832_PROG6212_POE.Data;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HR")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            

            return View();                            
        }

        public async Task<IActionResult> GenerateStatisticsReport(int? timeframe = null)
        {
            var reportData = new ReportData(_dbContext, _userManager);

            var lecturerStats = await reportData.LecturerStatisticsData(timeframe);
            var approverStats = await reportData.ApproverStatisticsData(timeframe);
            var hrStats = await reportData.HrStatisticsData(timeframe);
            var report = new StatisticsReportDocument(lecturerStats, approverStats, hrStats, timeframe);
            var fileBytes = report.GeneratePdf();
            return File(fileBytes, "application/pdf");
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\