using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinecraftBackend.Data;
using MinecraftBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinecraftBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GameDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(GameDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public class RegisterRequest
    {
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int GameModeId { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Players.AnyAsync(p => p.Email == request.Email || p.Code == request.Code))
            return BadRequest("Email hoặc Code đã tồn tại");

        var player = new Player
        {
            Code = request.Code,
            Email = request.Email,
            Password = request.Password,
            ExperiencePoints = 0,
            GameModeId = request.GameModeId
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return Ok("Đăng ký thành công");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p =>
            p.Email == request.Email && p.Password == request.Password);

        if (player == null)
            return Unauthorized("Sai email hoặc mật khẩu");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, player.Email),
            new Claim("code", player.Code),
            new Claim("gameModeId", player.GameModeId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpireMinutes"]!)
            ),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiresIn = _config["Jwt:ExpireMinutes"],
            user = new
            {
                player.Id,
                player.Code,
                player.Email,
                player.ExperiencePoints,
                player.GameModeId
            }
        });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p =>
            p.Password == request.OldPassword);

        if (player == null)
            return BadRequest("Mật khẩu cũ không đúng");

        player.Password = request.NewPassword;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
            return NotFound();

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
