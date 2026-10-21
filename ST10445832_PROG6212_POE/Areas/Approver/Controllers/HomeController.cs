// References
// https://geeksarray.com/blog/how-to-use-areas-in-asp-net-core-mvc

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Areas.Approver.Controllers
{
    [Area("Approver")]
    [Authorize(Roles = "Approver")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // redirect based on sub-role
            if (User.IsInRole("Programme Coordinator"))
            {
                return RedirectToAction(nameof(Index), "VerifyClaims");
            }
            else if (User.IsInRole("Academic Manager"))
            {
                return RedirectToAction(nameof(Index), "ApproveClaims");
            }
            return View();
        }

        public IActionResult MyAccount()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\