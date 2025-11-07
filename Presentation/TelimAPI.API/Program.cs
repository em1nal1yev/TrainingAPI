
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TelimAPI.Domain.Entities;
using TelimAPI.Domain.Enums;
using TelimAPI.Persistence;
using TelimAPI.Persistence.Contexts;
namespace TelimAPI.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddPersistenceServices(builder.Configuration);

            builder.Services.AddDistributedMemoryCache(); // Session üçün lazım olan arxa yaddaşı təmin edir (sadə testlər üçün)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Sessionun ömrü
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login"; // Login səhifənizin yolu
                options.AccessDeniedPath = "/Auth/AccessDenied"; // Yetkisizlik səhifəsi
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie ömrü
                options.SlidingExpiration = true; // Hər istifadədə yenilənmə
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("TrainerOrAdmin", policy => policy.RequireRole("Admin", "Trainer"));
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedAdminAsync(services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();




            app.MapControllers();


           

            app.Run();

            static async Task SeedAdminAsync(IServiceProvider serviceProvider)
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                string[] roles = Enum.GetNames(typeof(UserRole));

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>
                        {
                            Id = Guid.NewGuid(),
                            Name = role,
                            NormalizedName = role.ToUpper()
                        });
                    }
                }

                string adminEmail = "eminal1y3v@gmail.com";
                string adminPassword = "Admin123!";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new User
                    {
                        Name = "Admin",
                        Surname = "System",
                        Email = adminEmail,
                        UserName = adminEmail,
                        EmailConfirmed = true,
                        CourtId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        DepartmentId = Guid.Parse("10000000-0000-0000-0000-000000000001")
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
        }
    }
}
