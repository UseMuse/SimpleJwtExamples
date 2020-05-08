using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]SignInModel singIn)
        {
            SignInModel signInModel = new SignInModel()
            {
                Login = singIn.Login,
                Password = singIn.Password
            };
            IActionResult response = new UnauthorizedResult();
            var user = AuthenticateUser(signInModel);
            if (user != null)
            {
                var tokenStr = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }
        private SignInModel AuthenticateUser(SignInModel login)
        {
            SignInModel user = null;
            if (login.Login == "test" && login.Password == "test")
            {
                user = login;
            }
            return user;
        }
        private string GenerateJSONWebToken(SignInModel login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var now = DateTime.UtcNow;
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, login.Login),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var LifetimeInMinutes = int.Parse(_configuration["JwtSettings:LifetimeInMinutes"]);
            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(LifetimeInMinutes),
                signingCredentials: credentials
            );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }
    }
}
