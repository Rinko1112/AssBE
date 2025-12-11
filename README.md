# MinecraftBackend API (GAM106 Assignment - Y2 & Y3)

Dự án này cài đặt CSDL và API cho game giống Minecraft theo yêu cầu Y2 và Y3.

## Cấu trúc bảng chính (khoảng 5 bảng, liên kết ERD)

- GameModes (Id, Name)
- Players (Id, Code, Email, Password, ExperiencePoints, GameModeId)
- Resources (Id, Name, Description, Type)
- Items (Id, Name, Description, ImageUrl, Price, Category, IsWeapon)
- Purchases (Id, PlayerId, ItemId, PurchasedAt)

Các quan hệ:
- GameMode 1 - n Player
- Player 1 - n Purchase
- Item 1 - n Purchase

## Hướng dẫn chạy

1. Mở thư mục `MinecraftBackend` trong VS Code.
2. Kiểm tra chuỗi kết nối trong `appsettings.json` (ConnectionStrings:DefaultConnection) cho đúng SQL Server của bạn.
3. Chạy lệnh:
   - `dotnet restore`
   - `dotnet ef migrations add InitialCreate`
   - `dotnet ef database update`
4. Chạy project:
   - `dotnet run`
5. Mở Swagger:
   - Truy cập `https://localhost:5001/swagger` hoặc theo URL hiển thị trong console.

## Mapping API với yêu cầu Y3

1. Lấy tất cả tài nguyên: `GET /api/resources`
2. Lấy tất cả người chơi theo chế độ: `GET /api/players/by-mode?mode=Sinh tồn`
3. Lấy tất cả vũ khí có giá trị > 100: `GET /api/items/weapons?minPrice=100`
4. Lấy item người chơi có thể mua với XP hiện tại: `GET /api/players/{id}/affordable-items`
5. Lấy item tên chứa 'kim cương' và giá < 500: `GET /api/items/search?keyword=kim cương&maxPrice=500`
6. Lấy giao dịch mua item & phương tiện của 1 người chơi: `GET /api/players/{id}/purchases`
7. Thêm item mới: `POST /api/items`
8. Cập nhật mật khẩu người chơi: `PUT /api/players/{id}/password`
9. Lấy danh sách item được mua nhiều nhất: `GET /api/items/top-purchased?top=5`
10. Lấy tất cả người chơi và số lần mua: `GET /api/players/purchase-counts`