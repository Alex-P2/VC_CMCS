// References
// https://geeksarray.com/blog/how-to-use-areas-in-asp-net-core-mvc

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ST10445832_PROG6212_POE.Models;
using System.Diagnostics;

namespace ST10445832_PROG6212_POE.Areas.Lecturer.Controllers
{
    [Area("Lecturer")]
    [Authorize(Roles = "Lecturer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //=============================================================================================================================//
        /// <summary>
        /// simple redirect action to handle toasts on the index
        /// view after creating a claim
        /// </summary>
        public IActionResult ClaimCreated(bool success, string toastMessage)
        {
            if (success)
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = true, ToastMessage = toastMessage });
            }
            else
            {
                TempData["ToastData"] = JsonConvert.SerializeObject(new ToastPartialData { ToastType = false, ToastMessage = toastMessage });
            }
            return RedirectToAction("Index");
        }

        //=============================================================================================================================//
        /// <summary>
        /// User account view
        /// </summary>
        public IActionResult MyAccount()
        {
            return View();
        }


        //=============================================================================================================================//
        /// <summary>
        /// User account edit view
        /// </summary>
        [HttpPost]
        public IActionResult UpdateAccount()
        {
            return RedirectToAction("Index");
        }

        //=============================================================================================================================//
        /// <summary>
        /// Error view
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\