namespace MinecraftBackend.Models;

public class GameMode
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Player> Players { get; set; } = new List<Player>();
}