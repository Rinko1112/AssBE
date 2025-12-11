using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinecraftBackend.Data;
using MinecraftBackend.Models;

namespace MinecraftBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly GameDbContext _context;

    public PlayersController(GameDbContext context)
    {
        _context = context;
    }

    // GET: api/players/by-mode?mode=Sinh tồn
    [HttpGet("by-mode")]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayersByMode([FromQuery] string mode)
    {
        if (string.IsNullOrWhiteSpace(mode))
        {
            return BadRequest("Mode is required");
        }

        var players = await _context.Players
            .Include(p => p.GameMode)
            .Where(p => p.GameMode!.Name == mode)
            .AsNoTracking()
            .ToListAsync();

        return players;
    }

    // GET: api/players/{id}/affordable-items
    [HttpGet("{id:int}/affordable-items")]
    public async Task<ActionResult<IEnumerable<Item>>> GetAffordableItems(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        var items = await _context.Items
            .Where(i => i.Price <= player.ExperiencePoints)
            .AsNoTracking()
            .ToListAsync();

        return items;
    }

    // GET: api/players/{id}/purchases
    [HttpGet("{id:int}/purchases")]
    public async Task<ActionResult<IEnumerable<object>>> GetPurchasesForPlayer(int id)
    {
        var exists = await _context.Players.AnyAsync(p => p.Id == id);
        if (!exists)
        {
            return NotFound();
        }

        var query = await _context.Purchases
            .Where(p => p.PlayerId == id)
            .Include(p => p.Item)
            .OrderByDescending(p => p.PurchasedAt)
            .Select(p => new
            {
                p.Id,
                p.PlayerId,
                ItemName = p.Item!.Name,
                p.Item!.Category,
                p.Item!.Price,
                p.PurchasedAt
            })
            .ToListAsync();

        return query;
    }

    public class UpdatePasswordRequest
    {
        public string NewPassword { get; set; } = null!;
    }

    // PUT: api/players/{id}/password
    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
    {
        var player = await _context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest("Password is required");
        }

        player.Password = request.NewPassword;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/players/purchase-counts
    [HttpGet("purchase-counts")]
    public async Task<ActionResult<IEnumerable<object>>> GetPlayerPurchaseCounts()
    {
        var data = await _context.Players
            .Select(p => new
            {
                p.Id,
                p.Code,
                p.Email,
                PurchaseCount = p.Purchases.Count
            })
            .OrderByDescending(x => x.PurchaseCount)
            .ToListAsync();

        return data;
    }
}