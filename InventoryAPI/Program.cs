//using Microsoft.OpenApi.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using InventoryAPI.Data;
//using InventoryAPI.Models;
//using InventoryAPI.Repositories;
//using InventoryAPI.Services;

//var builder = WebApplication.CreateBuilder(args);



//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

//// Configure Swagger with JWT support
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });

//    // Add JWT Authentication to Swagger
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//// Configure Database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Configure Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//// Get JWT settings from configuration
//var jwtIssuer = builder.Configuration["JWT:ValidIssuer"];
//var jwtAudience = builder.Configuration["JWT:ValidAudience"];
//var jwtSecret = builder.Configuration["JWT:Secret"];

//// Debug output
//Console.WriteLine($"JWT Issuer: {jwtIssuer}");
//Console.WriteLine($"JWT Audience: {jwtAudience}");
//Console.WriteLine($"JWT Secret: {(string.IsNullOrEmpty(jwtSecret) ? "NOT FOUND" : "Found (length: " + jwtSecret.Length + ")")}");

//// Validate JWT configuration
//if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtSecret))
//{
//    Console.WriteLine("WARNING: JWT configuration is missing. Using default values for development.");
//    jwtIssuer ??= "http://localhost:5004";
//    jwtAudience ??= "http://localhost:4200";
//    jwtSecret ??= "DefaultSecretKeyForDevelopmentPurposesOnly12345!";
//}

//// Configure JWT Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtIssuer,
//        ValidAudience = jwtAudience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
//    };
//});

//// Configure AutoMapper
//builder.Services.AddAutoMapper(typeof(Program));

//// Register Repositories
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//// Register Services
//builder.Services.AddScoped<IAuthService, AuthService>();

//// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        builder =>
//        {
//            builder.AllowAnyOrigin()
//                   .AllowAnyMethod()
//                   .AllowAnyHeader();
//        });
//});

//// Add logging
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

//var app = builder.Build();


////===========================

//// Debug configuration
//Console.WriteLine("\n=== Configuration Debug ===");
//var configuration = app.Configuration;
//foreach (var pair in configuration.AsEnumerable())
//{
//    if (pair.Key.Contains("JWT") || pair.Key.Contains("ConnectionStrings"))
//    {
//        Console.WriteLine($"{pair.Key} = {pair.Value}");
//    }
//}
//Console.WriteLine("===========================\n");

////===============




//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API V1");
//    });
//}

//app.UseCors("AllowAll");
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

//// Create default admin user
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//        // Create Admin role
//        if (!await roleManager.RoleExistsAsync("Admin"))
//        {
//            await roleManager.CreateAsync(new IdentityRole("Admin"));
//            Console.WriteLine("? Admin role created");
//        }

//        // Create User role
//        if (!await roleManager.RoleExistsAsync("User"))
//        {
//            await roleManager.CreateAsync(new IdentityRole("User"));
//            Console.WriteLine("? User role created");
//        }

//        // Create admin user
//        var adminEmail = "admin@inventory.com";
//        var adminUser = await userManager.FindByEmailAsync(adminEmail);
//        if (adminUser == null)
//        {
//            adminUser = new ApplicationUser
//            {
//                UserName = adminEmail,
//                Email = adminEmail,
//                FirstName = "System",
//                LastName = "Administrator",
//                EmailConfirmed = true
//            };

//            var result = await userManager.CreateAsync(adminUser, "Admin@123");
//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(adminUser, "Admin");
//                Console.WriteLine("? Admin user created successfully!");
//                Console.WriteLine("  Email: admin@inventory.com");
//                Console.WriteLine("  Password: Admin@123");
//            }
//            else
//            {
//                Console.WriteLine($"? Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//            }
//        }
//        else
//        {
//            Console.WriteLine("? Admin user already exists");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"? Error during database initialization: {ex.Message}");
//    }
//}

//Console.WriteLine("\n? Application started successfully!");
//Console.WriteLine($"  API URL: http://localhost:5004");
//Console.WriteLine($"  Swagger: http://localhost:5004/swagger");
//Console.WriteLine($"  Angular App: http://localhost:4200\n");

//app.Run();


using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.Repositories;
using InventoryAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add performance optimizations
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// Configure DbContext with pooling for better performance
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(3);
            sqlOptions.CommandTimeout(30);
        }));

// Configure Identity with options
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication with caching
var jwtIssuer = builder.Configuration["JWT:ValidIssuer"] ?? "http://localhost:5004";
var jwtAudience = builder.Configuration["JWT:ValidAudience"] ?? "http://localhost:4200";
var jwtSecret = builder.Configuration["JWT:Secret"] ?? "DevelopmentSecretKey12345678901234567890";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero // Reduce clock skew for faster validation
    };
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();

// Create default admin user in background (don't block startup)
Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        var adminEmail = "admin@inventory.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating admin user: {ex.Message}");
    }
});

Console.WriteLine($"API running on http://localhost:5004");
app.Run();
