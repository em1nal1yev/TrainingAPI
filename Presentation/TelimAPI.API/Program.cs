
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using TelimAPI.Domain.Entities;
using TelimAPI.Domain.Enums;
using TelimAPI.Infrastructure;
using TelimAPI.Infrastructure.Options;
using TelimAPI.Persistence;
using TelimAPI.Persistence.Contexts;
using TelimAPI.Persistence.Options;
using TelimAPI.Persistence.Services.BackgroundServices;
namespace TelimAPI.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
           


            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            //builder.Services.AddDistributedMemoryCache(); 
            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30); 
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});


            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1); 
            });

            var jwtSettings = builder.Configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>();


            builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Sub
        };
    });

            builder.Services.AddAuthorization();

          

            builder.Services.AddControllers();



            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv6", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "Jwt",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });




            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Auth/Login"; 
            //    options.AccessDeniedPath = "/Auth/AccessDenied"; 
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); 
            //    options.SlidingExpiration = true; 
            //});



            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("all", builder => {
                    builder.WithOrigins("http://localhost:5198")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
            builder.Services.AddHostedService<TrainingStatusUpdater>();
            //builder.Services.AddHostedService<TrainingCleanupService>();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedAdminAsync(services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("all");


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
