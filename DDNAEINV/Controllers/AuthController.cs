using DDNAEINV.Data;
using DDNAEINV.Helper;
using DDNAEINV.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DDNAEINV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext dBContext;

        public AuthController(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthRequest auth)
        {
            if (auth == null)
            {
                return BadRequest(new { message = "Invalid request." });
            }

            var hasher = new PasswordHasher();

            var account = await dBContext.UserAccounts.FirstOrDefaultAsync(x => EF.Functions.Collate(x.UserName, "SQL_Latin1_General_CP1_CS_AS") == auth.UserName);


            if (account == null)
                return BadRequest(new { message = "User is not Authenticated!" });
            if (!hasher.VerifyPassword(account.Password, auth.Password))
                return Unauthorized(new { message = "Either Username and Password is Invalid!" });

            if (account.isVerified == false)
                return BadRequest(new { message = "Unverified Account, Contact Administrator!" });


            var userGroup = dBContext.UserGroups.Find(account.UGID);

            account.Token = createToken( account.UserName, userGroup!.UserGroupName);

            return Ok(new
            {
                Token = account.Token,
                UserID = account.UserID,
                UGID = account.UGID,
                message = "Welcome to DDN AEINV!!!"
            });
        }


        private String createToken(String username, String role)
        {


            var secretKEy = new PasswordHasher();
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("zxcvbnmasdfghjklqwertyuiop123456789");// Secret Key
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, username)

            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
