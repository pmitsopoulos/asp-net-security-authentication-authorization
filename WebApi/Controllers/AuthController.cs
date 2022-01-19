using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration config;

        public AuthController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate ([FromBody] Credential credential)
        {
            //Validating Credentials
            if (credential.UserName == "admin" && credential.Password == "pass")
            {

                #region Token Generation
                //with the Tokens we gett authenticated and then the only thing that is needed to generate a token is the claims that we have for a certain identity

                //Claims for the particular identity
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@mywebsite.com"),
                    new Claim("Department","HR"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate", "01-01-2021")
                };

              

                //The identity and principal are not needed when authenticating with tokens
                //Because we only need the claims on jwt auth
                //var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                //ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                
                var expiration = DateTime.UtcNow.AddMinutes(1);

                return Ok(new
                {
                    access_token = CreateToken(claims,expiration),
                    expires_at = expiration
                });
            }

            //Error message when credential validation fails
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endppoint.");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims,DateTime expiresAt )
        {
            var secret = Encoding.ASCII.GetBytes(config.GetValue<string>("SecretKey"));

            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        #endregion
    }

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
