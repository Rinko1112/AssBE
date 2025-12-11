namespace MinecraftBackend.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Price { get; set; } // experience points cost
    public string Category { get; set; } = "Item"; // Item or Vehicle
    public bool IsWeapon { get; set; }

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}