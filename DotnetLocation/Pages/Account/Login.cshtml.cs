using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorAuth.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, false);

            if (result.Succeeded)
                return RedirectToPage("/Index");

            ErrorMessage = "Invalid login";
            return Page();
        }
    }

    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
