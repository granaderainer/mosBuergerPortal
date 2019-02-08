namespace mosPortal.Models
{
    public class Login
    {
        /*[BindProperty]
        public LoginData loginData { get; set; }
        public async Task OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var isValid = (loginData.Username == "username" && loginData.Password == "password");
                if (!isValid)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return Page();
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, loginData.Username));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = loginData.RememberMe });
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("", "username or password is blank");
                return Page();
            }
        }
        public class LoginData
        {
            [Required]
            [Display(Name = "Benutzername")]
            public String Username { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Passwort")]
            public String Password { get; set; }

            [Required]
            [Display(Name = "Angemeldet bleiben?")]
            public Boolean RememberMe { get; set; }

            public LoginData(string username, string password, bool rememberMe)
            {
                this.Username = username;
                this.Password = password;
                this.RememberMe = rememberMe;
            }
        }*/
    }
}