using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

public static class StartupExtensions
{
    public static void ConfigureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<ISeedDataService, StoreSeedService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IStoreService, StoreService>();
        builder.Services.AddTransient<ICartService, CartService>();

        builder.Services.AddControllers();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDbContext<StoreDbContext>(opt => opt.UseInMemoryDatabase("Store"));
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
            };
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/SkuApi.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Logging.AddSerilog(logger);
        builder.Services.AddSingleton<Serilog.ILogger>(logger);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }


    public static void ConfigureApp(this WebApplication app)
    {
        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.AddLoginAndUserEndpoints();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            using (var scope = app.Services.CreateScope())
            {
                var seedDataService = scope.ServiceProvider.GetService<ISeedDataService>();
                seedDataService?.SeedDatabase();
            }
        }
    }
}