namespace MinecraftBackend.Models;

public class Purchase
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public DateTime PurchasedAt { get; set; }
}