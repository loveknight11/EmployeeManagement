using EmployeeManagement.Models;
using EmployeeManagement.ViewModels.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
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
            EditRoleViewModel model = new EditRoleViewModel
            {
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
                    Username = user.UserName,
                    UserId = user.Id,
                    IsInRole = usersInRole.Contains(user)
                });
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
            return RedirectToAction("EditRole", "administration", new { id = id });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {id}";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var claims = await userManager.GetClaimsAsync(user);

            var model = new EditUserViewModel
            {
                City = user.City,
                Email = user.Email,
                Id = user.Id,
                Username = user.UserName,
                Claims = claims.Select(x => x.Value).ToList(),
                Roles = roles.ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {Id}";
                return View("NotFound");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("listusers", "administration");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("listusers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.Error = $"Can't find role with Id = {Id}";
                return View("NotFound");
            }
            try
            {
                var result = await roleManager.DeleteAsync(role);

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
                return View("listroles");
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Title = $"{role.Name} is in use";
                ViewBag.Message = $"You can't delete {role.Name} Role while it has users";
                return View("NotFound");
            }

        }


        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            ViewBag.Id = id;
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {id}";
                return View("NotFound");
            }

            var roles = new List<UserRolesViewModel>();
            foreach (var role in roleManager.Roles)
            {
                roles.Add(new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(List<UserRolesViewModel> model, string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {id}";
                return View("NotFound");
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, userRoles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't remove user roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't add roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", "Administration", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> ManageClaims(string id)
        {

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {id}";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var allClaims = ClaimsStore.AllClaims;
            var model = new ManageClaimsViewModel() { UserId = user.Id };

            foreach (var claim in allClaims)
            {
                model.Claims.Add(new UserClaim
                {
                    ClaimType = claim.Type,
                    IsSelected = userClaims.Select(x => x.Type).Contains(claim.Type)
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageClaims(ManageClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.Error = $"Can't find user with Id = {model.UserId}";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, userClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't remove user Claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user, model.Claims.Where(x => x.IsSelected).Select(x => new Claim(x.ClaimType, x.ClaimType)));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Can't add claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", "Administration", new { id = model.UserId });
        }
    }
}
