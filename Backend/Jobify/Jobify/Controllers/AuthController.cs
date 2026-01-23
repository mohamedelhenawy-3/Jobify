using Application.DTOs.Auth;
using Application.Interfaces.Auth;
using Application.Interfaces.Token;
using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly ITokenService _tokenservice;
        public AuthController(IAuthService authService, ITokenService tokenservice) 
        {
            _authService = authService;
            _tokenservice = tokenservice;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);
            return Ok("Registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginreq)
        {

            var result = await _authService.LoginAsync(loginreq);
            return Ok(result);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            await _tokenservice.RefreshToken(dto.RefreshToken, dto.DeviceId);
            return Ok("Registered successfully");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var result = await _tokenservice.Logout(dto.RefreshToken, dto.DeviceId);
            return Ok(result);
        }
    }
}
