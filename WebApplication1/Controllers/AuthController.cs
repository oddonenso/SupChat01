using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Tables;
using ServerChat.Models;
using System.Threading.Tasks;

namespace ServerChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Connection _context;

        public AuthController(Connection context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Userss.AnyAsync(u => u.Login == user.Login))
            {
                return BadRequest("Пользователь уже существует.");
            }

            _context.Userss.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Пользователь успешно зарегистрирован.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel loginDetails)
        {
            var user = await _context.Userss
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == loginDetails.Login && u.Password == loginDetails.Password);

            if (user == null)
            {
                return Unauthorized("Неверные учетные данные.");
            }

            return Ok(new
            {
                UserId = user.UserID,
                UserName = $"{user.Name} {user.Surname}",
                Role = user.Role?.RoleName
            });
        }
    }
}
