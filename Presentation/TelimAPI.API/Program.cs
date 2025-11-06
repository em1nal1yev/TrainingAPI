
using Microsoft.AspNetCore.Identity;
using TelimAPI.Domain.Entities;
using TelimAPI.Persistence;
using TelimAPI.Persistence.Contexts;
using TelimAPI.Persistence.Seed;
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSession();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                // UserManager və RoleManager servislərini DI konteynerindən alırıq
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                try
                {
                    // Seeding metodunu çağırırıq
                    await ApplicationDbSeeder.SeedAsync(userManager, roleManager);
                    Console.WriteLine("İlkin rollar və admin istifadəçisi uğurla yaradıldı.");
                }
                catch (Exception ex)
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Verilənlər bazasının ilkin doldurulması zamanı xəta baş verdi.");
                }
            }

            app.Run();
        }
    }
}
