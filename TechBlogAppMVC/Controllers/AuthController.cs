using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechBlogAppMVC.DTOs;
using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {

            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var checkEmail = await _userManager.FindByEmailAsync(login.Email);
            if(checkEmail == null)
            {
                ViewBag.Error = "This email is not exist!";
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(checkEmail, login.Password, isPersistent:login.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("Error", "Email or Password is invalid!");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var checkemail = await _userManager.FindByEmailAsync(register.Email);
            if (checkemail != null)
            {
                return View();
            }

            User newUser = new()
            {
                Email = register.Email,
                UserName = register.Email,
                Name = register.Name,
                Surname = register.Surname,
                PhotoUrl = "/",
                AboutAuthor = ""
            };

            var result = await _userManager.CreateAsync(newUser, register.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(register);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        // DTO - data transfer object
    }
}
