using AHY.Identity.Entities;
using AHY.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AHY.Identity.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new UserCreateModel()); ;
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Email = model.Email,
                    Gender = model.Gender,
                    UserName = model.UserName,

                };

                var identityResult = await _userManager.CreateAsync(user, model.Password); // Kullanıcıyı oluşturmak için.
                if (identityResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member"); // Kayıt olan kullanıcıya rolünü vermek için.
                    return RedirectToAction("Index");
                }
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }

        public IActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new UserSignInModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel model)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                   
                    var role = await _userManager.GetRolesAsync(user);
                    if (role.Contains("Admin"))
                    {
                        return RedirectToAction("AdminPanel");
                    }
                    else
                    {
                        return RedirectToAction("Panel");
                    }
                }
                else if (signInResult.IsLockedOut)
                {
                    var lockOutEnd =await _userManager.GetLockoutEndDateAsync(user);
                    var openTime = lockOutEnd.Value.UtcDateTime - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Hesabınız geçici süreyle askıya alınmıştır. {(openTime).Minutes} dakika  {openTime.Seconds} saniye sonra açılacaktır.");
                }
                else
                {
                    var message = string.Empty;
                    //var userControl = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        message = $"{(_userManager.Options.Lockout.MaxFailedAccessAttempts - failedCount)} kez daha girerseniz hesabınız geçici olarak kilitlenecektir.";
                    }
                    else
                    {
                        message = "Kullanıcı adı ve ya şifre hatalı";
                    }
                    ModelState.AddModelError("", message);

                }
            }
            return View();
        }

        //[Authorize(Roles ="Admin, Member")] => Rol bazlı authorize
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value; // Authontacitaion olan kullanıcının rol bilgisini görmek için..
            User.IsInRole("Member");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public IActionResult Panel()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public IActionResult MemberPage() => View();

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied() => View();
    }
}
