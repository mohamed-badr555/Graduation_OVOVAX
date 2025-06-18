using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.DTOs.Auth;
using OVOVAX.Core.Entities.Identity;
using OVOVAX.Core.Services.Contract;
using System.Security.Claims;

namespace OVOVAX.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);
            return Ok(result);
        }

        [HttpGet("validate-token")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> ValidateToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new AuthResponse 
                { 
                    Success = false, 
                    Error = "Token not provided" 
                });
            }

            var result = await _authService.ValidateTokenAsync(token);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new AuthResponse 
                { 
                    Success = false, 
                    Error = "User not found" 
                });
            }

            var token = await _authService.CreateTokenAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Token = token,
                LastLogin = user.LastLogin
            });
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _authService.CheckEmailExistsAsync(email);
        }

        [HttpPost("validate-password")]
        public ActionResult<PasswordValidationResult> ValidatePassword([FromBody] ValidatePasswordRequest request)
        {
            var result = _authService.ValidatePassword(request.Password);
            return Ok(result);
        }
    }

    // DTO for ForgotPassword request
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    // DTO for ValidatePassword request
    public class ValidatePasswordRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}