using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mosPortal.Models.ViewModels;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using mosPortal.Models;
using mosPortal.Data;

namespace mosPortal.Controllers
{
    public class AccountController : Controller
    {
        
        private dbbuergerContext db = new dbbuergerContext();
        private SignInManager<User> signInManager;
        private UserManager<User> userManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signManager)
        {
            this.userManager = userManager;
            this.signInManager = signManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return PartialView("LoginView", model: model);
        }
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new RegisterViewModel();
            return PartialView("RegisterView", model: model);

        }
        [HttpPost]
        public async Task<IActionResult> Register (RegisterViewModel model, string returnUrl = "")
        {
            //TODo: Beide PW gleich --> Check
            var userCheck = db.User.Where(u => u.UserName == model.Username).SingleOrDefault();
            var addressCheck = db.Address.Where(a => a.Country == model.Country && a.City == model.City && a.ZipCode == model.ZipCode && a.Street == model.Street && a.Number == model.Number).SingleOrDefault();
            if (addressCheck == null)
            {
                Address newAddress = new Address
                {
                    Country = model.Country,
                    City = model.City,
                    ZipCode = model.ZipCode,
                    Street = model.Street,
                    Number = model.Number
                };
                db.Add(newAddress);
                db.SaveChanges();
            }
            addressCheck = db.Address.Where(a => a.Country == model.Country && a.City == model.City && a.ZipCode == model.ZipCode && a.Street == model.Street && a.Number == model.Number).SingleOrDefault();
            
            if (userCheck == null)
            {
                User newUser = new User
                {
                    Firstname = model.Firstname,
                    Name = model.Name,
                    UserName = model.Username,
                    Email = model.Email,
                    Birthday = model.Birthday.ToString(),
                    Birthplace = model.Birthplace,
                    AddressId = addressCheck.Id
                };
                IdentityResult identityResult = await signInManager.UserManager.CreateAsync(newUser,model.Password);
                if (identityResult.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        //return Redirect(returnUrl);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid register attempt");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username,
                   model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
    }
}