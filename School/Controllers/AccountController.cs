using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace School.Controllers
{
    public class AccountController : Controller
    {
        readonly UserManager<IdentityUser> userManager;
        readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string repassword)
        {
            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Password don't match");

                return View();
            }

            var newUser = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            try
            {
                var userCreationResult = await userManager.CreateAsync(newUser, password);
                if (!userCreationResult.Succeeded)
                {
                    foreach (var error in userCreationResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return View();
                }

                ModelState.AddModelError(string.Empty, "User created!");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            return View();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login");
                    return View();
                }

                var passwordSignInResult = await signInManager.PasswordSignInAsync(user, password, isPersistent: rememberMe, lockoutOnFailure: false);
                if (!passwordSignInResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login");
                    return View();
                }

                return Redirect("~/");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("~/");
        }
    }
}
