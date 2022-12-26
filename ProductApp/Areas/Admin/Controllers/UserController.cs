using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProductApp.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task <IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        [HttpGet]
        public async Task <IActionResult> AddRoleToUser(string id)
        {
            
            var user = await _userManager.FindByNameAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles,"Id","Name");
            
            ViewBag.RoleList = roles;
            return View(user);
        }
        [HttpPost]
        public async Task <IActionResult> AddRoleToUser(string username,string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            var rol = await _roleManager.FindByIdAsync(role);
            var flag = await _userManager.IsInRoleAsync(user, rol.Name);
            if (flag == true)
            {
                await _userManager.RemoveFromRoleAsync(user, rol.Name);
                return RedirectToAction("Index");
            }

            if (user is not null && rol is not null ){
                var result = await _userManager.AddToRoleAsync(user, rol.Name);

            
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);

                    }
                } 
        }
            return RedirectToAction("Index");
        }
    }
}
