using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Fiorella.Models;
using Fiorella.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fiorella.Controllers
{
    public class Account : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public Account(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            };
            var existUser = await _userManager.FindByNameAsync(registerViewModel.Username);
            if (existUser !=null)
            {
                ModelState.AddModelError("Username","Bu adda User var");
                return View();
            }

            var user = new User
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Username,
                Fullname = registerViewModel.Fullname
            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                    return View();
                }
            }
            
            
            await _signInManager.SignInAsync(user,false);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Verify(string email,string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            TempData["confirmed"] = true;

            return RedirectToAction(nameof(Index),"Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existUser = await _userManager.FindByNameAsync(loginViewModel.Username);
            if (existUser==null)
            {
                ModelState.AddModelError("","Invalid Credentials");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, loginViewModel.Password, loginViewModel.RememberMe, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("","Invalid Credentials");
                return View();
            }
            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> ResetPassword(string token,string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            string mailtoken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(Verify), "Account", new { email = user.Email, mailtoken }, Request.Scheme, Request.Host.ToString());

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
            msg.To.Add(user.Email);
            string body = string.Empty;
            using(StreamReader reader = new StreamReader("wwwroot/template/verifyemail.html"))
            {
                body = reader.ReadToEnd();
            }
            msg.Body = body.Replace("{{link}}", link);
            msg.Subject = "Verify";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);
            TempData["confirm"] = true;
            if (token==null || email==null)
            {
                ModelState.AddModelError("","Invalid Credentials");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user==null)
            {
                ModelState.AddModelError("Email","Bu emailde user movcud deyl");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token,
                resetPasswordViewModel.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordComfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }
            return View(resetPasswordViewModel);
        }
    }
}