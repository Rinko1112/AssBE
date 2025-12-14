using Microsoft.EntityFrameworkCore;
using MinecraftBackend.Models;

namespace MinecraftBackend.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<GameMode> GameModes => Set<GameMode>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Purchase> Purchases => Set<Purchase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<GameMode>().HasData(
        new GameMode { Id = 1, Name = "Sinh tồn" },
        new GameMode { Id = 2, Name = "Sáng tạo" },
        new GameMode { Id = 3, Name = "Phiêu lưu" },
        new GameMode { Id = 4, Name = "Khán giả" },
        new GameMode { Id = 5, Name = "Hardcore" } // ✅ thêm cho đủ 5
    );

    modelBuilder.Entity<Player>(entity =>
    {
        entity.HasIndex(p => p.Code).IsUnique();   // ✅ UNIQUE Code
        entity.HasIndex(p => p.Email).IsUnique();  // ✅ UNIQUE Email

        entity.HasOne(p => p.GameMode)
              .WithMany(g => g.Players)
              .HasForeignKey(p => p.GameModeId);
    });

    modelBuilder.Entity<Player>().HasData(
        new Player { Id = 1, Code = "PL001", Email = "player1@example.com", Password = "123456", ExperiencePoints = 800, GameModeId = 1 },
        new Player { Id = 2, Code = "PL002", Email = "player2@example.com", Password = "123456", ExperiencePoints = 300, GameModeId = 1 },
        new Player { Id = 3, Code = "PL003", Email = "player3@example.com", Password = "123456", ExperiencePoints = 1200, GameModeId = 2 },
        new Player { Id = 4, Code = "PL004", Email = "player4@example.com", Password = "123456", ExperiencePoints = 450, GameModeId = 3 },
        new Player { Id = 5, Code = "PL005", Email = "player5@example.com", Password = "123456", ExperiencePoints = 600, GameModeId = 1 }
    );

    modelBuilder.Entity<Resource>().HasData(
        new Resource { Id = 1, Name = "Gỗ", Description = "Tài nguyên cơ bản để xây dựng", Type = "Gỗ" },
        new Resource { Id = 2, Name = "Đá", Description = "Dùng để chế tạo công cụ", Type = "Khoáng sản" },
        new Resource { Id = 3, Name = "Sắt", Description = "Khoáng sản dùng để chế tạo vũ khí", Type = "Khoáng sản" },
        new Resource { Id = 4, Name = "Vàng", Description = "Khoáng sản quý", Type = "Khoáng sản" },
        new Resource { Id = 5, Name = "Kim cương", Description = "Tài nguyên hiếm", Type = "Khoáng sản" }
    );

    modelBuilder.Entity<Item>()
    .ToTable(t => t.HasCheckConstraint("CK_Item_Price", "[Price] >= 0"));


    modelBuilder.Entity<Item>().HasData(
        new Item { Id = 1, Name = "Kiếm gỗ", Description = "Vũ khí cơ bản", Price = 50, Category = "Item", IsWeapon = true },
        new Item { Id = 2, Name = "Kiếm kim cương", Description = "Vũ khí mạnh", Price = 400, Category = "Item", IsWeapon = true },
        new Item { Id = 3, Name = "Áo giáp kim cương", Description = "Giáp bảo vệ mạnh", Price = 450, Category = "Item", IsWeapon = false },
        new Item { Id = 4, Name = "Ngựa chiến", Description = "Phương tiện di chuyển", Price = 300, Category = "Vehicle", IsWeapon = false },
        new Item { Id = 5, Name = "Thuyền gỗ", Description = "Phương tiện di chuyển trên nước", Price = 150, Category = "Vehicle", IsWeapon = false },
        new Item { Id = 6, Name = "Cuốc sắt", Description = "Công cụ khai thác", Price = 120, Category = "Item", IsWeapon = false }
    );

    modelBuilder.Entity<Purchase>().HasData(
        new Purchase { Id = 1, PlayerId = 1, ItemId = 2, PurchasedAt = DateTime.UtcNow.AddDays(-5) },
        new Purchase { Id = 2, PlayerId = 1, ItemId = 4, PurchasedAt = DateTime.UtcNow.AddDays(-4) },
        new Purchase { Id = 3, PlayerId = 2, ItemId = 1, PurchasedAt = DateTime.UtcNow.AddDays(-3) },
        new Purchase { Id = 4, PlayerId = 3, ItemId = 3, PurchasedAt = DateTime.UtcNow.AddDays(-2) },
        new Purchase { Id = 5, PlayerId = 3, ItemId = 2, PurchasedAt = DateTime.UtcNow.AddDays(-1) }
    );
}

}