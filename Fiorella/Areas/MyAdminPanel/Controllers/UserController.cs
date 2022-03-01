using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorella.DataLayerAccess;
using Fiorella.Models;
using Fiorella.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fiorella.Areas.MyAdminPanel.Controllers
{
    [Area("MyAdminPanel")]

    public class UserController:Controller
    {
        
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(AppDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            List<UserViewModel> userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()??"Doesn't Have Role",
                    IsActive = user.IsActive
                };
                userViewModels.Add(userViewModel);
            }
            return View(userViewModels);
        }

        public async Task<IActionResult> Activate(string id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user==null)
            {
                return NotFound();
            }

            
            if (user.IsActive==true)
            {
                user.IsActive = false;
            }
            else
            {
                user.IsActive = true;
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (role==null)
            {
                return BadRequest();
            }
            ViewBag.CurrentRole = role;

            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangeRole(string id,string NewRole)
        {
            if (id==null && NewRole == null)
            {
                return  NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user==null)
            {
                return NotFound();
            }
            string exRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (exRole == null)
            {
                return NotFound();
            }
            if (exRole!= NewRole)
            {
                var result= await _userManager.AddToRoleAsync(user, NewRole);
                if (!result.Succeeded)
                {

                    ModelState.AddModelError("", "This is Problem");
                  
                }
                var RemoveRole = await _userManager.RemoveFromRoleAsync(user, exRole);
                if (RemoveRole.Succeeded)
                {
                    ModelState.AddModelError("", "This is problem");
                }
                await _userManager.UpdateAsync(user);

            }

            return RedirectToAction(nameof(Index));

        }
        
        public IActionResult ChangePassword(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }
            var user = await _userManager.FindByIdAsync(changePasswordViewModel.Id);
            if (user == null)
            {
                return BadRequest();
            }
            var checkCurrentPass = await _userManager.CheckPasswordAsync(user, changePasswordViewModel.CurrentPassword);
            if (checkCurrentPass==false)
            {
                ModelState.AddModelError("CurrentPassword", "duzgun sifre daxil et");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
           
            return RedirectToAction(nameof(Index));
        }
    }
}