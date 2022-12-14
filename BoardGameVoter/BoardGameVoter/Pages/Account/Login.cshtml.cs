using BoardGameVoter.Models.EntityModels.Users;
using BoardGameVoter.Pages.Shared;
using BoardGameVoter.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace BoardGameVoter.Pages.Account
{
    public class LoginModel : BoardGameVoterPageBase
    {

        public LoginModel(IBGVServiceProvider bGVServiceProvider)
            : base(bGVServiceProvider)
        {
        }

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (IsValidEmail(Email))
                {
                    User user = UserManager.FindByEmail(Email);

                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = SignInManager.PasswordSignIn(user, Password);
                    if (result.Succeeded)
                    {
                        Logger.LogInformation("User logged in.");
                        return LocalRedirect(ReturnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        Logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        [Required]
        [BindProperty]
        [Display(Name = "Email / Username")]
        public string Email { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [Required]
        [BindProperty]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        [BindProperty]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; } = "/";
    }
}

