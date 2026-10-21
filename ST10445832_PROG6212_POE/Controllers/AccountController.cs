// References
// https://geeksarray.com/blog/how-to-use-areas-in-asp-net-core-mvc

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10445832_PROG6212_POE.Models;

namespace ST10445832_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult SignIn()
        {
            // redirect to respective area home if already signed in
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("HR"))
                {
                    return RedirectToAction("Index", "Home", new { area = "HR" });
                }
                else if (User.IsInRole("Lecturer"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Lecturer" });
                }
                else if (User.IsInRole("Approver"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Approver" });
                }
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInCredentials signInCredentials)
        {
            if (!ModelState.IsValid)
            {
                return View("SignIn", signInCredentials);
            }

            var user = await _userManager.FindByEmailAsync(signInCredentials.Email);
            if (user is not null)
            {
                var response = await _signInManager.PasswordSignInAsync(user, signInCredentials.Password, false, false);
                if (response.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("HR"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "HR" });
                    }
                    else if (roles.Contains("Lecturer"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Lecturer" });
                    }
                    else if (roles.Contains("Approver"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Approver" });
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Passoword Incorrect");
                    return View("SignIn", signInCredentials);
                }
            }
            ModelState.AddModelError("Email", "Email not found");
            return View("SignIn", signInCredentials);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Account");
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\