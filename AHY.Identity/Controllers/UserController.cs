using AHY.Identity.Context;
using AHY.Identity.Entities;
using AHY.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AHY.Identity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AHYContext _context;
        private readonly RoleManager<AppRole> _roleManager;
        public UserController(UserManager<AppUser> userManager, AHYContext context, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            #region User List With Join Query
            //IQueryable => Sorgunun daha veritabanıyla işi bitmemiş. Yani daha toList edilmemiş.
            //var query = _userManager.Users;
            //var list = _context.Users.Join(_context.UserRoles, user => user.Id, userRole => userRole.UserId, (user, userrole) => new
            //{
            //    user,
            //    userrole
            //}).Join(_context.Roles, two => two.userrole.RoleId, role => role.Id, (two, role) => new { two.user, two.userrole, role }).Where(x => x.role.Name != "Admin").Select(x => new AppUser
            //{
            //    Id = x.user.Id,
            //    AccessFailedCount = x.user.AccessFailedCount,
            //    ConcurrencyStamp = x.user.ConcurrencyStamp,
            //    Email = x.user.Email,
            //    EmailConfirmed = x.user.EmailConfirmed,
            //    Gender = x.user.Gender,
            //    ImagePath = x.user.ImagePath,
            //    LockoutEnabled = x.user.LockoutEnabled,
            //    LockoutEnd = x.user.LockoutEnd,
            //    NormalizedEmail = x.user.NormalizedEmail,
            //    NormalizedUserName = x.user.NormalizedUserName,
            //    PasswordHash = x.user.PasswordHash,
            //    PhoneNumber = x.user.PhoneNumber,
            //    PhoneNumberConfirmed = x.user.PhoneNumberConfirmed,
            //    SecurityStamp = x.user.SecurityStamp,
            //    TwoFactorEnabled = x.user.TwoFactorEnabled,
            //    UserName = x.user.UserName

            //}).ToList();
            #endregion
            //var users = await _userManager.GetUsersInRoleAsync("Member");


            var filteredUser = new List<AppUser>();
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    filteredUser.Add(user);
                }
            }
            return View(filteredUser);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserAdminCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Gender = model.Gender,
                };

                var result = await _userManager.CreateAsync(user, "123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AssignRole(int id)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.Id == id);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.ToList();

            RoleAssignSendModel model = new();
            List<RoleAssignListModel> list = new();
            foreach (var item in roles)
            {
                list.Add(new()
                {
                    Name = item.Name,
                    RoleId = item.Id,
                    Exist = userRoles.Contains(item.Name)
                });
            }

            model.Roles = list;
            model.UserId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignSendModel model)
        {
            //rol ekleme => seçilen rolün olmaması
            //rol çıkarma => seçilen rolün olması

            var user = _userManager.Users.SingleOrDefault(x => x.Id == model.UserId);
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var item in model.Roles)
            {
                if (item.Exist)
                {
                    if (!userRoles.Contains(item.Name))
                    {
                        await _userManager.AddToRoleAsync(user, item.Name);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, item.Name);
                    }
                }
            }
            return RedirectToAction("Index", "User");
        }
    }
}
