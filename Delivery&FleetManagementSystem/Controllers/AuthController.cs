using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.AuthenticationServices;
using System.Security.Claims;
using SystemDTOS.AuthenticationDTOS;
namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]RegisterDTO dto)
        {
            var user = await _authService.RegisterAsync(dto);
            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<AuthDTO> Login([FromBody]LoginDTO dto)
        {
            AuthDTO auth = _authService.Login(dto);
            return Ok(auth);

        }
        [Authorize]
        [HttpGet]
        public ActionResult GetMe()
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = _authService.GetMe(userID);
            return Ok(user);
        }
        


    }
}
