using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinecraftBackend.Data;
using MinecraftBackend.Models;

namespace MinecraftBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly GameDbContext _context;

    public ItemsController(GameDbContext context)
    {
        _context = context;
    }

    // GET: api/items/weapons?minPrice=100
    [HttpGet("weapons")]
    public async Task<ActionResult<IEnumerable<Item>>> GetWeapons([FromQuery] int minPrice = 100)
    {
        var items = await _context.Items
            .Where(i => i.IsWeapon && i.Price > minPrice)
            .AsNoTracking()
            .ToListAsync();

        return items;
    }

    // GET: api/items/search?keyword=kim cương&maxPrice=500
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Item>>> SearchItems([FromQuery] string keyword, [FromQuery] int maxPrice)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest("Keyword is required");
        }

        keyword = keyword.ToLower();

        var items = await _context.Items
            .Where(i => i.Name.ToLower().Contains(keyword) && i.Price < maxPrice)
            .AsNoTracking()
            .ToListAsync();

        return items;
    }

    // POST: api/items
    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    // GET: api/items/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetItemById(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        return item;
    }

    // GET: api/items/top-purchased?top=5
    [HttpGet("top-purchased")]
    public async Task<ActionResult<IEnumerable<object>>> GetTopPurchasedItems([FromQuery] int top = 5)
    {
        var query = await _context.Items
            .OrderByDescending(i => i.Purchases.Count)
            .Take(top)
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Price,
                i.Category,
                PurchaseCount = i.Purchases.Count
            })
            .ToListAsync();

        return query;
    }
}