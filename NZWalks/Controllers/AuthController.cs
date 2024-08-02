using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NZWalks.CustomActionFilters;
using NZWalks.Models.DTO;
using NZWalks.Repositories;
using PostmarkDotNet;

namespace NZWalks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PostmarkClient _postmarkClient; // Add PostmarkClient
    private readonly IOptions<PostmarkSettings> _postmarkSettings;
    private readonly IConfiguration _configuration; // Define a field to hold the IConfiguration instance

    public AuthController(
        UserManager<IdentityUser> userManager,
        ITokenRepository tokenRepository,
        PostmarkClient postmarkClient,
        IOptions<PostmarkSettings> postmarkSettings,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
        _postmarkClient = postmarkClient; // Initialize PostmarkClient
        _postmarkSettings = postmarkSettings;
        _configuration = configuration; // Assign the injected IConfiguration to the field
    }

    //POST: https://localhost:7103/api/auth/Register
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var identityUser = new IdentityUser()
        {
            UserName = registerRequestDto.Username,
            Email = registerRequestDto.Username
        };
        var identityResult = await _userManager.CreateAsync(
            identityUser,
            registerRequestDto.Password
        );
        if (identityResult.Succeeded)
            //Add roles to this user
            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await _userManager.AddToRolesAsync(
                    identityUser,
                    registerRequestDto.Roles
                );
                if (identityResult.Succeeded)
                    return Ok("User created successfully!");
            }

        return BadRequest("Something went wrong! Please try again.");
    }

    //POST: https://localhost:7103/api/auth/Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginRequestDto.Username);
        if (
            identityUser != null
            && await _userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password)
        )
        {
            //get roles for this user
            var roles = await _userManager.GetRolesAsync(identityUser);
            if (roles != null)
            {
                //create JWT token
                var jwtToken = _tokenRepository.CreateJWTToken(identityUser, roles.ToList());
                var response = new LoginResponseDto() { JwtToken = jwtToken };
                return Ok(response);
            }
        }

        return BadRequest("Invalid login attempt");
    }

    //POST: https://localhost:7103/api/auth/RequestResetPassword
    [HttpPost]
    [Route("RequestResetPassword")]
    public async Task<IActionResult> RequestResetPassword(
        [FromBody] ResetPasswordRequestDto request
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Username);
        if (user == null)
        {
            // Log this attempt, return a generic response for security
            return Ok("If your email is registered, you will receive a password reset link.");
        }
        var clientUrl = _configuration["ClientUrl"]; // Access a configuration value
        // var callbackUrl = $"{clientUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";


        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // var callbackUrl = $"{_configuration["ClientUrl"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
        var callbackUrl =
            $"{clientUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
        var message = new PostmarkMessage()
        {
            To = user.Email,
            From = _postmarkSettings.Value.SenderEmail,
            Subject = "Reset Your Password",
            HtmlBody = $"Reset your password by <a href='{callbackUrl}'>clicking here</a>."
        };

        await _postmarkClient.SendMessageAsync(message);

        return Ok("If your email is registered, you will receive a password reset link.");
    }
}
