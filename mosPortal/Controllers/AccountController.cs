using System.Linq;
using System.Threading.Tasks;
using mosPortal.Data;
using mosPortal.Models;
using mosPortal.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace mosPortal.Controllers
{
    public class AccountController : Controller
    {
        //Attribute
        private readonly dbbuergerContext db;
        private readonly SignInManager<User> signInManager;
        private UserManager<User> userManager;

        //Konstruktor
        public AccountController(UserManager<User> userManager, SignInManager<User> signManager, dbbuergerContext db)
        {
            this.userManager = userManager;
            signInManager = signManager;
            this.db = db;
        }
        //Loginseite
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            LoginViewModel model = new LoginViewModel { ReturnUrl = returnUrl };
            return PartialView("_Modal", model);
        }
        //Registrierungsseite
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            RegisterViewModel model = new RegisterViewModel();
            return PartialView("RegisterView", model);
        }
        //Registrierung
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = "")
        {
            //TODo: Beide PW gleich --> Check
            //Überprüfen, ob User und Adresse bereits existieren
            User userCheck = db.User.Where(u => u.UserName == model.Username).SingleOrDefault();
            Address addressCheck = db.Address.Where(a =>
                a.Country == model.Country && a.City == model.City && a.ZipCode == model.ZipCode &&
                a.Street == model.Street && a.Number == model.Number).SingleOrDefault();
            //Gültigkeit des Registierungskeys Prüfen
            try
            {
                Randomkey key = db.Randomkey.Where(r => r.Key == model.Registerkey).First();
                db.Randomkey.Remove(key);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("Registererror",
                    "Registerkey is not available, maybe this key is not valid anymore");
                return View("RegisterView", model);
            }

            //Falls Adresse nicht vorhanden, neue Anlegen
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

            addressCheck = db.Address.Where(a =>
                a.Country == model.Country && a.City == model.City && a.ZipCode == model.ZipCode &&
                a.Street == model.Street && a.Number == model.Number).SingleOrDefault();

            //Falls User nicht vorhanden, neuen User anlegen
            if (userCheck == null)
            {
                User newUser = new User
                {
                    Firstname = model.Firstname,
                    Name = model.Name,
                    UserName = model.Username,
                    Email = model.Email,
                    Birthday = model.Birthday,
                    Birthplace = model.Birthplace,
                    AddressId = addressCheck.Id
                };
                //User Speichern und Passwort Hashen
                IdentityResult identityResult = await signInManager.UserManager.CreateAsync(newUser, model.Password);

                //Falls Speicherung erfolgreich wird User der Rolle "User" zugeordnet
                if (identityResult.Succeeded)
                {
                    bool register = false;
                    //await signInManager.UserManager.AddToRoleAsync(newUser, "User"); Es wird IsInRoleAsync aufgerufen
                    Role role = db.Role.SingleOrDefault(r => r.Name == "User");
                    UserRole userRole = new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = role.Id
                    };
                    db.Add(userRole);
                    db.SaveChanges();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        register = true;
                        return RedirectToAction("Index", "Home");
                        //return Redirect(returnUrl);
                    }

                    register = false;
                    return RedirectToAction("Index", "Home", new { register });
                }
            }


            ModelState.AddModelError("", "Invalid register attempt");
            return View("RegisterView", model);
        }
        //Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            bool login = false;
            if (ModelState.IsValid)
            {
                //Anmeldung des Benutzers
                SignInResult result = await signInManager.PasswordSignInAsync(model.Username,
                    model.Password, model.RememberMe, false);
                //Falls Anmeldung erfolgreich, wird er auf die Index Seite weitergeleitet
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    login = true;
                    return RedirectToAction("Index", "Home", new { login });
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View("_Modal", model);
        }
        //Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //Abmeldung des Users und Weiterleitug zur Startseite
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}