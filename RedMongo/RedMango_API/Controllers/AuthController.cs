using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.Dto;
using RedMango_API.Utility;
using BCrypt.Net;

namespace RedMango_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ApiResponse _response;
        private readonly string _secretKey;

        public AuthController(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _secretKey = configuration.GetSection("ApiSettings:Secret").Value;
            _response = new ApiResponse();
        }

       [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO model)
        {
            var userFromDb = _db.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());

            if (userFromDb == null || !BCrypt.Net.BCrypt.Verify(model.Password, userFromDb.Password))
            {
                _response.Result = new LoginResponseDTO();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { "Invalid credentials" };
                return BadRequest(_response);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey.Trim());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("fullname", userFromDb.Name),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim("email", userFromDb.Email),
                    new Claim(ClaimTypes.Role, userFromDb.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var claimsPrincipal = tokenHandler.ValidateToken(tokenString, validationParameters, out _);

                var loginResponse = new LoginResponseDTO
                {
                    Email = userFromDb.Email,
                    Token = tokenString
                };

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = loginResponse;
                return Ok(_response);
            }
            catch (SecurityTokenException ex)
            {
                _response.Result = null;
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { "Token validation failed: " + ex.Message };
                return Unauthorized(_response);
            }
        }


        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDTO model)
        {
            var userFromDb = _db.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());

            if (userFromDb != null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { "User already exists" };
                return BadRequest(_response);
            }
            var newUser = new User
            {
                Email = model.Email,
                Name = model.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role?.ToLower() == "admin" ? Role.Admin : Role.User
            };

            try
            {
                _db.Users.Add(newUser);
                _db.SaveChanges();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessage.Add(ex.Message);
                return BadRequest(_response);
            }
        }
    }
}
