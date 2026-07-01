using Microsoft.EntityFrameworkCore;
using WebAccGameMVC.Models;
using WebAccGameMVC1.Data;

namespace WebAccGameMVC.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            // Use migrations to update database schema. EnsureCreated() bypasses migrations
            // and will break migration-based workflows. Use Migrate() so pending
            // migrations are applied automatically at startup.
            context.Database.Migrate();

            Console.WriteLine("Bắt đầu seed dữ liệu mới...");

            var categories = new Category[]
            {
                new Category { CategoryName = "Hành động" },
                new Category { CategoryName = "Phiêu lưu" },
                new Category { CategoryName = "Thể thao" },
                new Category { CategoryName = "Nhập vai (RPG)" },
                new Category { CategoryName = "Chiến thuật" },
                new Category { CategoryName = "Kinh dị" },
                new Category { CategoryName = "Giả lập (simulator)" },
                new Category { CategoryName = "Thế giới mở" }
            };

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
            else
            {
                categories = context.Categories.ToArray();
            }

            var products = new Product[]
{
                // Bánh kem
                new Product
                {
                    ProductName = "Ark Survival Evolved 7 DLC – Tài Khoản Steam Full Thông Tin",
                    Price = 50000,
                    Description = "ARK: Survival Evolved là tựa game sinh tồn thế giới mở đầy phiêu lưu được phát triển bởi Studio Wildcard. Người chơi có thể tải game ARK: Survival Evolved trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam Full thông tin tại Tedi Shop. Phát hành chính thức vào ngày 29 tháng 8 năm 2017 sau giai đoạn Early Access, game hiện đã có mặt trên các nền tảng Windows, PlayStation 4, Xbox One, Nintendo Switch, iOS và Android. Phiên bản nâng cấp ARK: Survival Ascended đã ra mắt vào tháng 10 năm 2023, mang đến đồ họa và trải nghiệm được cải tiến đáng kể.",
                    Image = "/images/ark.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Black Myth: Wukong",
                    Price = 79000,
                    Description = "Black Myth: Wukong là tựa game hành động nhập vai đình đám được phát triển bởi Game Science, một studio game độc lập đến từ Trung Quốc. Người chơi có thể tải game Black Myth: Wukong trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline. Phát hành chính thức vào ngày 20 tháng 8 năm 2024, game đã có mặt trên các nền tảng PlayStation 5, PC (Steam, Epic Games Store) và Xbox Series X/S.",
                    Image = "/images/wk.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Red Dead Redemption 2",
                    Price = 49000,
                    Description = "Red Dead Redemption 2 là tựa game hành động phiêu lưu thế giới mở được phát triển và phát hành bởi Rockstar Games. Người chơi có thể tải game Red Dead Redemption 2 trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline tại Tedi Shop. Phát hành chính thức vào ngày 26 tháng 10 năm 2018 trên PlayStation 4 và Xbox One, sau đó ra mắt trên PC vào ngày 5 tháng 11 năm 2019 và Stadia vào ngày 19 tháng 11 năm 2019.",
                    Image = "/images/reddead.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Elden Ring",
                    Price = 400000,
                    Description = "Elden Ring là tựa game nhập vai hành động thế giới mở được phát triển bởi FromSoftware và phát hành bởi Bandai Namco Entertainment. Người chơi có thể tải game Elden Ring trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline tại Tedi Shop. Phát hành chính thức vào ngày 25 tháng 2 năm 2022, game đã có mặt trên các nền tảng PlayStation 4, PlayStation 5, Xbox One, Xbox Series X/S và Windows PC (Steam).",
                    Image = "/images/eldering.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Ghost of Tsushima DIRECTOR’S CUT",
                    Price = 69000,
                    Description = "Ghost of Tsushima DIRECTOR’S CUT là phiên bản hoàn chỉnh nhất của tựa game hành động phiêu lưu đình đám được phát triển bởi Sucker Punch Productions và phát hành bởi Sony Interactive Entertainment. Người chơi có thể tải game Ghost of Tsushima DIRECTOR’S CUT trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline. Phát hành chính thức vào ngày 20 tháng 8 năm 2021 cho PlayStation 5 và PlayStation 4, game đã chính thức có mặt trên PC (Steam) vào ngày 16 tháng 5 năm 2024.",
                    Image = "/images/godoftushima.png",
                    CategoryId = categories[0].CategoryId
                },

                // Bánh bông lan
                new Product
                {
                    ProductName = "Marvel’s Spider-Man 2 Digital Deluxe",
                    Price = 79000,
                    Description = "Marvel’s Spider-Man 2 – Digital Deluxe là phiên bản cao cấp của tựa game hành động phiêu lưu đình đám được phát triển bởi Insomniac Games. Người chơi có thể tải game Marvel’s Spider-Man 2 trực tiếp PlayStation Store hoặc trải nghiệm qua tài khoản Steam offline. Phát hành chính thức vào ngày 20 tháng 10 năm 2023, game hiện đã có mặt độc quyền trên nền tảng PlayStation 5 và PC (Steam, Epic Games Store).",
                    Image = "/images/spiderman2.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Assassin’s Creed Shadows",
                    Price = 69000,
                    Description = "Assassin’s Creed Shadows là phiên bản game hành động nhập vai mới nhất trong series Assassin’s Creed danh tiếng được phát triển bởi Ubisoft Quebec. Người chơi có thể tải game Assassin’s Creed Shadows trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline. Dự kiến phát hành chính thức vào ngày 20 tháng 3 năm 2025, game sẽ có mặt trên các nền tảng PlayStation 5, Xbox Series X/S và PC. Đáng chú ý, game cũng sẽ hỗ trợ macOS và iPadOS.",
                    Image = "/images/assassins.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "God of War Ragnarok",
                    Price = 50000,
                    Description = "God of War Ragnarok là phiên bản tiếp theo của tựa game hành động phiêu lưu đình đám được phát triển bởi Santa Monica Studio và phát hành bởi Sony Interactive Entertainment. Người chơi có thể tải game God of War Ragnarok trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline. Game đã có mặt trên các nền tảng Windows PC thông qua Steam.",
                    Image = "/images/godofwar.png",
                    CategoryId = categories[0].CategoryId
                },

                // Bánh quy
                new Product
                {
                    ProductName = "Cyberpunk 2077",
                    Price = 100000,
                    Description = "Cyberpunk 2077 + DLC Phantom Liberty là tựa game hành động nhập vai góc nhìn thứ nhất được phát triển bởi CD Projekt RED. Người chơi có thể tải game Cyberpunk 2077 + DLC Phantom Liberty trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline tại Tedi Shop. Phát hành chính thức vào năm 2020 và DLC Phantom Liberty ra mắt sau đó, game hiện đã có mặt trên các nền tảng PlayStation 5, Xbox Series X/S và PC.",
                    Image = "/images/cyperpunk.png",
                    CategoryId = categories[1].CategoryId
                },
                new Product
                {
                    ProductName = "DRAGON BALL: Sparking! ZERO Ultimate Edition",
                    Price = 95000,
                    Description = "Dragon Ball: Sparking! ZERO là phiên bản mới nhất trong series game đối kháng Dragon Ball Budokai Tenkaichi được phát triển bởi Spike Chunsoft và phát hành bởi Bandai Namco Entertainment. Người chơi có thể tải game Dragon Ball: Sparking! ZERO trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam offline. Dự kiến phát hành chính thức vào quý 4 năm 2024, game sẽ có mặt trên các nền tảng PlayStation 5, Xbox Series X/S và PC (Steam).",
                    Image = "/images/dragonball.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "PRAGMATA",
                    Price = 100000,
                    Description = "PRAGMATA là tựa game hành động – phiêu lưu khoa học viễn tưởng do Capcom phát triển, lấy bối cảnh trong một tương lai gần nơi công nghệ đã vượt ngoài tầm kiểm soát.\r\n\r\nCâu chuyện xoay quanh một phi hành gia và một cô bé bí ẩn mang sức mạnh AI, cùng nhau tìm cách sống sót và khám phá sự thật trên một trạm nghiên cứu bị bỏ hoang trên Mặt Trăng. Trong hành trình này, họ phải phối hợp để vượt qua các mối nguy hiểm và giải mã những bí mật đằng sau thảm họa công nghệ.",
                    Image = "/images/pragmata.png",
                    CategoryId = categories[0].CategoryId
                },new Product
                {
                    ProductName = "The Last of Us II Remastered",
                    Price = 79000,
                    Description = "Game mang đến trải nghiệm hành động phiêu lưu sâu sắc với bối cảnh Hoa Kỳ hậu tận thế, cho phép người chơi điều khiển hai nhân vật chính: Ellie, người lên đường trả thù cho một vụ giết người, và Abby, một người lính bị cuốn vào cuộc xung đột giữa tổ chức quân sự của cô và một giáo phái tôn giáo. Người chơi sẽ phải chiến đấu với kẻ thù là con người và những sinh vật giống zombie bằng súng, vũ khí tự chế và lẩn trốn.",
                    Image = "/images/tlou.png",
                    CategoryId = categories[0].CategoryId
                },
                new Product
                {
                    ProductName = "Left 4 Dead 2",
                    Price = 40000,
                    Description = "Left 4 Dead 2 là tựa game bắn súng góc nhìn thứ nhất co-op được phát triển bởi Valve Corporation. Người chơi có thể tải game Left 4 Dead 2 trực tiếp Steam hoặc trải nghiệm qua tài khoản Steam Full thông tin tại Tedi Shop. Phát hành chính thức vào ngày 17 tháng 11 năm 2009, game hiện đã có mặt trên các nền tảng Windows, macOS, Linux và Xbox 360. Mặc dù đã ra mắt hơn một thập kỷ, Left 4 Dead 2 vẫn được cập nhật thường xuyên và duy trì cộng đồng người chơi đông đảo.",
                    Image = "/images/left4.png",
                    CategoryId = categories[0].CategoryId
                }
            };

            if (!context.Products.Any())
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }
            else
            {
                products = context.Products.ToArray();
            }

          

            Console.WriteLine("Seed dữ liệu hoàn tất!");
        }
    }
}
