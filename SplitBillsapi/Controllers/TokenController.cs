using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SplitBillsapi.Helpers;

namespace SplitBillsapi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1.0/[controller]")]
    [Authorize]
    public class TokenController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        private IConfiguration _config;
        private ExceptionHandler _exceptionHandler;
        public TokenController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            try
            {
                var user = Authenticate(login);
                if (user != null)
                {
                    var tokenString = BuildToken(user);
                    response = Ok(new { token = tokenString });
                }
            }
            catch(Exception ex)
            {
                _exceptionHandler.ErrorCode = "1000";
                _exceptionHandler.ErrorMessage = ex.Message;
                return BadRequest(_exceptionHandler);
            }
            return response;
        }
        private LoginModel Authenticate(LoginModel login)
        {
            LoginModel loginModel = null;
            if (!(login.SignInName.Trim().Equals(string.Empty)) && !(login.PassKey.Trim().Equals(string.Empty)))
            {
                loginModel = login;
            }
            return loginModel;
        }
        private string BuildToken(LoginModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new[] { new Claim("Role", "SignInUser") };
                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                            _config["Jwt:Audience"],
                            claims,
                            expires: DateTime.Now.AddMinutes(30),
                            signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}