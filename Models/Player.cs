namespace MinecraftBackend.Models;

public class Player
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int ExperiencePoints { get; set; }

    public int GameModeId { get; set; }
    public GameMode? GameMode { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}