using MaslomatAPI.Models;
using MaslomatAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaslomatAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] UserRequest request)
        {
            var response = _service.Authenticate(request);

            if (response == null)
                return BadRequest(new { message = "Bad username or password" });

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRequest request)
        {
            var user = _service.Register(request);
            if (user == null)
                return BadRequest(new { message = "Username already taken." });

            return Ok(new { message = $"User with name {user.Name} succesfully registered!", token = user.Token });
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _service.GetAllUsers();
            return Ok(users);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("passwords")]
        public IActionResult GetAllPasswords()
        {
            var passwords = _service.GetAllPasswords();
            return Ok(passwords);
        }

        [Authorize]
        [HttpPost("passwords")]
        public IActionResult GetPasswords([FromBody] UserRequest user)
        {
            var passwords = _service.GetPasswords(user);

            if (passwords == null)
                return BadRequest(new { message = "Username or password is incorrect." });

            return Ok(passwords);
        }

        [Authorize]
        [HttpPost("add-password")]
        public IActionResult PostPassword([FromBody] PasswordsUpdateRequest request)
        {
            var password = _service.SavePassword(request);

            if (password == null)
                return NotFound(); //idk

            return Ok(new { message = $"Password saved!" });
        }

        [Authorize]
        [HttpPost("remove-password")]
        public IActionResult DeletePassword([FromBody] PasswordsUpdateRequest request)
        {
            var password = _service.DeletePassword(request);

            if (password == null)
                return NotFound(); //idk

            return Ok(new { message = $"Password removed!" });
        }
    }
}
