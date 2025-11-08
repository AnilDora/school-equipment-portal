using Microsoft.AspNetCore.Mvc;
using SchoolEquipmentApi.DTOs;
using SchoolEquipmentApi.Modules;

namespace SchoolEquipmentApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UsersController : ControllerBase
    {
        private readonly IDataStore _dataStore;

        public UsersController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "username and password required" });

            var data = _dataStore.Read();
            if (data.Users.Any(u => u.Username == dto.Username))
                return Conflict(new { message = "User exists" });

            var user = new User 
            { 
                Username = dto.Username, 
                Password = dto.Password, 
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "student" : dto.Role 
            };
            
            data.Users.Add(user);
            var token = Guid.NewGuid().ToString("N");
            data.Sessions[token] = new Session { Username = user.Username, Role = user.Role };
            _dataStore.Write(data);
            
            return Ok(new { token, user = new { user.Username, user.Role } });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "username and password required" });
                
            var data = _dataStore.Read();
            var found = data.Users.FirstOrDefault(u => u.Username == dto.Username && u.Password == dto.Password);
            
            if (found == null) 
                return Unauthorized();
                
            var token = Guid.NewGuid().ToString("N");
            data.Sessions[token] = new Session { Username = found.Username, Role = found.Role };
            _dataStore.Write(data);
            
            return Ok(new { token, user = new { found.Username, found.Role } });
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var token = Helpers.GetBearer(Request);
            var data = _dataStore.Read();
            
            if (token == null || !data.Sessions.TryGetValue(token, out var session)) 
                return Unauthorized();
                
            return Ok(new { user = session });
        }
    }
}