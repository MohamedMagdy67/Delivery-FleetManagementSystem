using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.AuthenticationServices;
using System.Security.Claims;
using SystemContext.SystemDbContext;
using SystemDTOS.AuthenticationDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailVerficationController : ControllerBase
    {
            private readonly EmailVerificationService _service;

            public EmailVerficationController(EmailVerificationService service)
            {
                _service = service;
            }

            [HttpGet("verify")]
            public async Task<IActionResult> Verify([FromQuery] string token)
            {
                await _service.VerifyEmailAsync(token);
                return Ok("Email verified successfully");
            }
            
            [HttpPost("resend")]
            public async Task<IActionResult> Resend([FromBody]string email)
            {
                await _service.ResendVerificationAsync(email);
                return Ok("Verification email sent");
            }
    }
 }

