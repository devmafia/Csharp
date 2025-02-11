using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Models;
using UserService.Dtos;
using UserService.Messaging;

namespace UserService.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IMessageBus _messageBus;

        public UserController(UserDbContext context, IMessageBus messageBus)
        {
            _context = context;
            _messageBus = messageBus;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserProfile userProfile)
        {
            if (await _context.Users.AnyAsync(u => u.Name == userProfile.Name))
            {
                return Conflict("User already exists.");
            }
            var new_userProfile = new UserProfile
            {
                Id = userProfile.Id,
                Name = userProfile.Name,
                Preferences = userProfile.Preferences
            };

            _context.Users.Add(new_userProfile);
            await _context.SaveChangesAsync();
            _messageBus.PublishUserEvent("USER_REGISTERED", new_userProfile);
            return CreatedAtAction(nameof(GetUserProfile), new { id = new_userProfile.Id }, userProfile);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string token)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var user = await _context.Users.FindAsync(decodedToken.Uid);
                if (user == null) return Unauthorized("User not found.");
                return Ok(new { UserId = decodedToken.Uid });
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok("User logged out successfully.");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UpdateUserProfile updatedProfile)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = updatedProfile.Name;
            user.Preferences = updatedProfile.Preferences;
            await _context.SaveChangesAsync();
            _messageBus.PublishUserEvent("USER_UPDATED", user);

            return NoContent();
        }

    }
}
