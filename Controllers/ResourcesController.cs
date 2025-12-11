using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinecraftBackend.Data;
using MinecraftBackend.Models;

namespace MinecraftBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourcesController : ControllerBase
{
    private readonly GameDbContext _context;

    public ResourcesController(GameDbContext context)
    {
        _context = context;
    }

    // GET: api/resources
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Resource>>> GetAllResources()
    {
        return await _context.Resources.AsNoTracking().ToListAsync();
    }
}