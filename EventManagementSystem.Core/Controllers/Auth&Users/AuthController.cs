using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.BLL.Services;
using EventManagementSystem.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using EventManagementSystem.API.DTOs.AuthDtos;
namespace EventManagementSystem.API.Controllers.Auth_Users
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly TokenService _JwtService;
		public AuthController(TokenService jwtService)
        {
            _JwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _JwtService.Authunticate(request);
            if(result == null) 
                return Unauthorized();
            return Ok(result);
        }


	}
}
