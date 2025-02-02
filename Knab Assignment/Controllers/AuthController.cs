using Knab.Assignment.API.DTOs;
using Knab.Assignment.API.Providers;
using Knab.Assignment.Domain;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Knab.Assignment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJWTTokenProvider _tokenProvider;

        public AuthController(ILogger<AuthController> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJWTTokenProvider tokenProvider)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO userRegistrationDto)
        {
            try
            {
                var userEntity = userRegistrationDto.Adapt<User>();
                userEntity.UserName = userEntity.Email;

                var result = await _userManager.CreateAsync(userEntity, userRegistrationDto.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to register.");
                return Problem(detail: "Registration failed!", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

                if (user == null)
                {
                    return Unauthorized("Invalid authorization!");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

                if (result.Succeeded)
                {
                    var token = _tokenProvider.GenerateToken(user);
                    _logger.LogInformation($"User {user.Id} logged in at {DateTime.UtcNow}.");

                    return Ok(token);
                }

                return Unauthorized("Invalid authorization!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to login.");
                return Problem(detail: "Login failed!", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
