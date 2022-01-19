using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp_UnderTheHood.Authorization;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential credential { get; set; }
        public void OnGet()
        {
           
        }
        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            //Verify the given Credentials
            if(credential.UserName=="admin" && credential.Password == "pass")
            {

                #region Security Context Generation
                //Generate the Security Context

                //Claims for the particular identity
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@mywebsite.com"),
                    new Claim("Department","HR"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate", "01-01-2021")
                };

                //Assign the claims to our identity
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                #endregion
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return RedirectToPage("/Index");

            }
            return Page(); 
        }
        
    }
}
