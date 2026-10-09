using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace AxiomSysSL.Controllers;

[ApiController]
[Route("api/auth")]
[ApiExplorerSettings(GroupName = "Sales")]
public sealed class AuthController(
    IConfiguration config, IWebHostEnvironment environment) : ControllerBase
{
    public sealed record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // This issuer is only for a local assessment. Never enable it in production.
        if (!environment.IsDevelopment()) return NotFound();

        var username = config["DemoAuth:Username"];
        var password = config["DemoAuth:Password"];
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return StatusCode(503);

        var userMatches = string.Equals(request.Username, username,
            StringComparison.Ordinal);
        var passwordMatches = CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(request.Password ?? "")),
            SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        if (!userMatches || !passwordMatches) return Unauthorized();

        var userId = config.GetValue<long>("DemoAuth:UserId");
        if (userId <= 0 || userId > 9_999_999_999L) return StatusCode(503);
        var company = config["Assessment:CompanyCode"]!;
        var expires = DateTime.UtcNow.AddMinutes(30);
        var claims = new[]
        {
            new Claim("Id", userId.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            new Claim("name", username),
            new Claim("CompanyCode", company),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256));
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiresUtc = expires,
            username,
            companyCode = company
        });
    }
}
