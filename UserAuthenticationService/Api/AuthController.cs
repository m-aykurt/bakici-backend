using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAuthenticationService.ApiContract.Requests;
using UserAuthenticationService.ApiContract.Responses;
using UserAuthenticationService.ApplicationService.Commands;
using UserAuthenticationService.ApplicationService.Queries;

namespace UserAuthenticationService.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                UserType = request.UserType
            };

            try
            {
                var userId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetUserById), new { id = userId }, null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GetIpAddress()
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return Unauthorized(new AuthenticationResponse { Success = false, Message = result.Message });

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(MapToAuthenticationResponse(result));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request = null)
        {
            var refreshToken = request?.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var command = new RefreshTokenCommand
            {
                RefreshToken = refreshToken,
                IpAddress = GetIpAddress()
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return Unauthorized(new AuthenticationResponse { Success = false, Message = result.Message });

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(MapToAuthenticationResponse(result));
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
                return Ok(new { message = "No refresh token provided" });

            var userId = User.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Unauthorized(new { message = "Invalid token" });

            var command = new LogoutCommand
            {
                UserId = userGuid,
                RefreshToken = refreshToken,
                IpAddress = GetIpAddress()
            };

            await _mediator.Send(command);
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new ForgotPasswordCommand
            {
                Email = request.Email,
                ResetUrl = $"{Request.Scheme}://{Request.Host}/reset-password"
            };

            await _mediator.Send(command);

            // Always return OK to prevent email enumeration attacks
            return Ok(new { message = "If the email exists, a password reset link has been sent" });
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new ResetPasswordCommand
            {
                Token = request.Token,
                NewPassword = request.NewPassword
            };

            try
            {
                var result = await _mediator.Send(command);
                
                if (result)
                    return Ok(new { message = "Password has been reset successfully" });
                else
                    return BadRequest(new { message = "Failed to reset password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery { UserId = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(new UserResponse
            {
                Id = result.Id,
                Email = result.Email,
                FirstName = result.FirstName,
                LastName = result.LastName,
                PhoneNumber = result.PhoneNumber,
                UserType = result.UserType.ToString(),
                EmailVerified = result.EmailVerified,
                PhoneVerified = result.PhoneVerified,
                CreatedAt = result.CreatedAt,
                LastLoginAt = result.LastLoginAt,
                IsActive = result.IsActive,
                Roles = result.Roles
            });
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict,
                Secure = Request.IsHttps
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private AuthenticationResponse MapToAuthenticationResponse(ApplicationService.Models.AuthenticationResult result)
        {
            return new AuthenticationResponse
            {
                Success = result.Success,
                Message = result.Message,
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                Expiration = result.Expiration,
                User = result.User != null ? new UserResponse
                {
                    Id = result.User.Id,
                    Email = result.User.Email,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    PhoneNumber = result.User.PhoneNumber,
                    UserType = result.User.UserType.ToString(),
                    EmailVerified = result.User.EmailVerified,
                    PhoneVerified = result.User.PhoneVerified,
                    CreatedAt = result.User.CreatedAt,
                    LastLoginAt = result.User.LastLoginAt,
                    IsActive = result.User.IsActive,
                    Roles = result.User.Roles
                } : null
            };
        }
    }
} 