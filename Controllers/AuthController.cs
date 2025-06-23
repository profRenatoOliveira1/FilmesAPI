using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase {
    [HttpPost("login")]
    public IActionResult Login([FromBody] Usuario usuario) {
        if (usuario.Email != "admin@cinelog.com" || usuario.Senha != "admin123") {
            return Unauthorized(new { mensagem = "Credenciais inválidas. É necessário autenticar-se para acessar o sistema." });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UmAnelParaGovernar_Todos_123!EncontralosComSeguranca"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "FilmesAPI",
            audience: "FilmesAPI",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}
