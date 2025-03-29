using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FiapSub.Core.UseCases.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FiapSub.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthenticateUserUseCase _authenticateUserUseCase;

    public AuthController(AuthenticateUserUseCase authenticateUserUseCase)
    {
        _authenticateUserUseCase = authenticateUserUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authenticateUserUseCase.Execute(request.Email, request.Password);
            
            var token = GenerateJwtToken(result.UserId, result.UserType);
            
            return Ok(new {
                token,
                userId = result.UserId,
                userType = result.UserType
            });
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
    }

    private string GenerateJwtToken(int userId, string userType)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DV3QCJ2+ePuH1ke9YZFy9CCSrMIJdlQn3KziIH0jfXGZgWAXnWAJXTKNuzQfjuEu@"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("userType", userType),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5001",
            audience: "https://localhost:5001",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}