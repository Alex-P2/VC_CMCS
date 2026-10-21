using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10445832_PROG6212_POE.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HR")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\