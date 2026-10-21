using Microsoft.AspNetCore.Mvc;
using ST10445832_PROG6212_POE.Models;
using System.Diagnostics;

namespace ST10445832_PROG6212_POE.Areas.Lecturer.Controllers
{
    public class ErrorController : Controller
    {
        //=============================================================================================================================//
        /// <summary>
        /// Default constructor
        /// </summary>
        public ErrorController() { }

        [Route("Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = statusCode });
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\