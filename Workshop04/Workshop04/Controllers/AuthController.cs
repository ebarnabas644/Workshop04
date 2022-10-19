using Workshop04Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace M7CarManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;


        public AuthController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDataModel model)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            //Registering
            if (user == null)
            {
                var createUser = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await userManager.CreateAsync(createUser, model.Password);
                //await userManager.AddToRoleAsync(createUser, "Customer");

                return Ok();
            }
            //Logging in
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Email),
                };
                foreach (var role in await userManager.GetRolesAsync(user))
                {
                    claim.Add(new Claim(ClaimTypes.Role, role));
                }
                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("biztonsagostitkoskod"));
                var token = new JwtSecurityToken(
                    issuer: "http://www.security.org", audience: "http://www.security.org",
                    claims: claim, expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }

            return Unauthorized();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserInfos()
        {
            var user = userManager.Users.FirstOrDefault(t => t.Email == this.User.Identity.Name);
            if (user != null)
            {
                return Ok(new
                {
                    UserName = user.UserName,
                    Email = user.Email,

                    Roles = await userManager.GetRolesAsync(user)
                });
            }
            return Unauthorized();
        }

       
        

    }
}
