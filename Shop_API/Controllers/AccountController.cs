using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Shop_API.Data;
using Shop_API.Models;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shop_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DBContext _context;

        public AccountController(DBContext context)
        {
            _context = context;
        }

        public record regBody(string login, string password, string name);
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody]regBody regBody)
        {
            if (_context.User.Any(x => x.Login == regBody.login))
                return "Пользователь уже есть";
            

            var hmac = new HMACSHA512();
            var role = _context.Role.First(r => r.Name == "Клиент");

            var user = new User
            {
                Name = regBody.name,
                Login = regBody.login,
                RoleId = role.Id,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regBody.password)),
                PasswordSalt = hmac.Key,
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login),
                                           new Claim(ClaimTypes.Role, user.RoleId.ToString())};
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return "token " + new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public record loginBody(string login, string password);
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] loginBody body)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Login == body.login);

            if (user == null)
                return Unauthorized("Invalid login");

            if (!VerifyPasswordHash(body.password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid password");

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login),
                                           new Claim(ClaimTypes.Role, user.RoleId.ToString())};
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return "token " + new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }
            return true;
        }
    }
}

