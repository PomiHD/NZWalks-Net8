using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models.DTO;
using NZWalks.Models.Repositories;

namespace NZWalks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenRepository _tokenRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
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
        var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);
        if (identityResult.Succeeded)
            //Add roles to this user
            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                if (identityResult.Succeeded) return Ok("User created successfully!");
            }

        return BadRequest("Something went wrong! Please try again.");
    }

    //POST: https://localhost:7103/api/auth/Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginRequestDto.Username);
        if (identityUser != null && await _userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password))
        {
            //get roles for this user
            var roles = await _userManager.GetRolesAsync(identityUser);
            if (roles != null)
            {
                //create JWT token
                var jwtToken = _tokenRepository.CreateJWTToken(identityUser, roles.ToList());
                var response = new LoginResponseDto()
                {
                    JwtToken = jwtToken
                };
                return Ok(response);
            }
        }


        return BadRequest("Invalid login attempt");
    }
}