using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinecraftBackend.Data;
using MinecraftBackend.Models;

namespace MinecraftBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly GameDbContext _context;

    public PurchasesController(GameDbContext context)
    {
        _context = context;
    }

    // POST: api/purchases
    // Mỗi lần mua chỉ mua 1 item / phương tiện
    [HttpPost]
    public async Task<ActionResult<Purchase>> CreatePurchase([FromBody] Purchase purchase)
    {
        var player = await _context.Players.FindAsync(purchase.PlayerId);
        var item = await _context.Items.FindAsync(purchase.ItemId);

        if (player == null || item == null)
        {
            return BadRequest("Player or Item not found");
        }

        if (player.ExperiencePoints < item.Price)
        {
            return BadRequest("Không đủ điểm kinh nghiệm để mua item này.");
        }

        // Trừ điểm kinh nghiệm
        player.ExperiencePoints -= item.Price;

        purchase.PurchasedAt = DateTime.UtcNow;

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = purchase.Id }, purchase);
    }

    // GET: api/purchases/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Purchase>> GetById(int id)
    {
        var purchase = await _context.Purchases
            .Include(p => p.Item)
            .Include(p => p.Player)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (purchase == null)
        {
            return NotFound();
        }

        return purchase;
    }
}