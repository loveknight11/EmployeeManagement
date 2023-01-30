using EmployeeManagement.Models;
using EmployeeManagement.ViewModels.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = model.Name };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("listroles", "administration");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.Error = $"Role with Id = {id} is not found";
                return View("NotFound");
            }
            EditRoleViewModel model = new EditRoleViewModel { 
            Id = role.Id,
            RoleName = role.Name
            };
            var RoleUsers = await userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in RoleUsers)
            {
                model.Users.Add(user.UserName);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var Role = await roleManager.FindByIdAsync(model.Id);
            if (Role == null)
            {
                ViewBag.Error = $"can not find role with Id = {model.Id} to update";
                return View("NotFound");
            }
            else
            {
                Role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(Role);
                if (result.Succeeded)
                {
                    return RedirectToAction("listroles", "administration");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.Error = $"Role with Id = {id} is not found";
                return View("NotFound");
            }
            var users = userManager.Users;
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            List<RoleUsers> roleUsers = new List<RoleUsers>();
            foreach (var user in users)
            {
                roleUsers.Add(new RoleUsers()
                {
                    Username   = user.UserName,
                    UserId = user.Id,
                    IsInRole = usersInRole.Contains(user)
                }) ;
            }
            ViewBag.roleId = role.Id;
            ViewBag.roleName = role.Name;
            return View(roleUsers);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<RoleUsers> roleUsers, string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

            foreach (var user in roleUsers)
            {
                var appUser = await userManager.FindByIdAsync(user.UserId);
                IdentityResult result = new IdentityResult();
                if (user.IsInRole && !usersInRole.Contains(appUser))
                {
                    
                    result = await userManager.AddToRoleAsync(appUser, role.Name);
                }
                else if (!user.IsInRole && usersInRole.Contains(appUser))
                {
                    result = await userManager.RemoveFromRoleAsync(appUser, role.Name);
                }
            }
            return RedirectToAction("EditRole", "administration", new { id = id});
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }


    }
}
