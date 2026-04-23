
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

//// Add performance optimizations
//builder.Services.AddResponseCaching();
//builder.Services.AddMemoryCache();

//// Configure DbContext with pooling for better performance
//builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
//        sqlOptions =>
//        {
//            sqlOptions.EnableRetryOnFailure(3);
//            sqlOptions.CommandTimeout(30);
//        }));

//// Configure Identity with options
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    // Password settings
//    options.Password.RequireDigit = true;
//    options.Password.RequiredLength = 6;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;

//    // Lockout settings
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//    options.Lockout.MaxFailedAccessAttempts = 5;
//    options.Lockout.AllowedForNewUsers = true;

//    // User settings
//    options.User.RequireUniqueEmail = true;
//})
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();


//// Get JWT settings with fallbacks
//var jwtIssuer = builder.Configuration["JWT:ValidIssuer"] ?? builder.Configuration["Jwt:ValidIssuer"] ?? "http://localhost:5004";
//var jwtAudience = builder.Configuration["JWT:ValidAudience"] ?? builder.Configuration["Jwt:ValidAudience"] ?? "http://localhost:4200";
//var jwtSecret = builder.Configuration["JWT:Secret"] ?? builder.Configuration["Jwt:SecretKey"] ?? "DefaultSecretKeyForDevelopmentPurposesOnly12345!";

//Console.WriteLine($"JWT Issuer (using): {jwtIssuer}");
//Console.WriteLine($"JWT Audience (using): {jwtAudience}");
//Console.WriteLine($"JWT Secret length (using): {jwtSecret.Length}");


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;

//    // Get JWT settings
//    var jwtIssuer = builder.Configuration["JWT:ValidIssuer"] ?? builder.Configuration["Jwt:ValidIssuer"];
//    var jwtAudience = builder.Configuration["JWT:ValidAudience"] ?? builder.Configuration["Jwt:ValidAudience"];
//    var jwtSecret = builder.Configuration["JWT:Secret"] ?? builder.Configuration["Jwt:SecretKey"];

//    Console.WriteLine($"=== JWT Configuration Debug ===");
//    Console.WriteLine($"JWT Issuer: {jwtIssuer}");
//    Console.WriteLine($"JWT Audience: {jwtAudience}");
//    Console.WriteLine($"JWT Secret exists: {!string.IsNullOrEmpty(jwtSecret)}");
//    Console.WriteLine($"================================");


//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtIssuer,
//        ValidAudience = jwtAudience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
//        ClockSkew = TimeSpan.Zero
//    };


//    // Add event handlers for debugging token validation
//    options.Events = new JwtBearerEvents
//    {

//        OnMessageReceived = context =>
//        {
//            var authHeader = context.Request.Headers["Authorization"].ToString();
//            Console.WriteLine($"Raw Authorization Header: {authHeader}");

//            if (!string.IsNullOrEmpty(authHeader))
//            {
//                // Remove "Bearer " prefix if present
//                var token = authHeader;
//                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//                {
//                    token = token.Substring("Bearer ".Length).Trim();
//                }
//                context.Token = token;
//                Console.WriteLine($"Token extracted successfully. Length: {token.Length}");
//            }

//            return Task.CompletedTask;
//        },


//        OnAuthenticationFailed = context =>
//        {
//            Console.WriteLine($"Authentication Failed: {context.Exception.Message}");
//            return Task.CompletedTask;
//        },
//        OnTokenValidated = context =>
//        {
//            Console.WriteLine($"=== Token Validated Successfully ===");
//            return Task.CompletedTask;
//        },
//        OnChallenge = context =>
//        {
//            Console.WriteLine($"=== Challenge ===");
//            Console.WriteLine($"Error: {context.Error}");
//            Console.WriteLine($"Description: {context.ErrorDescription}");
//            return Task.CompletedTask;
//        }
//    };




//});

//// Add services
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddResponseCaching();
//builder.Services.AddResponseCompression(options =>
//{
//    options.EnableForHttps = true;
//});

//// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        builder =>
//        {
//            builder.AllowAnyOrigin()
//                   .AllowAnyMethod()
//                   .AllowAnyHeader()
//                   .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
//        });
//});

//var app = builder.Build();

//// Configure pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseCors("AllowAll");
//app.UseAuthentication();
//app.UseAuthorization();

//app.UseResponseCaching();
//app.MapControllers();
//app.UseResponseCompression();


//// Create default admin user in background (don't block startup)
////Task.Run(async () =>
////{
////    using var scope = app.Services.CreateScope();
////    var services = scope.ServiceProvider;
////    try
////    {
////        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
////        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

////        if (!await roleManager.RoleExistsAsync("Admin"))
////            await roleManager.CreateAsync(new IdentityRole("Admin"));

////        if (!await roleManager.RoleExistsAsync("User"))
////            await roleManager.CreateAsync(new IdentityRole("User"));

////        var adminEmail = "admin@inventory.com";
////        var adminUser = await userManager.FindByEmailAsync(adminEmail);
////        if (adminUser == null)
////        {
////            adminUser = new ApplicationUser
////            {
////                UserName = adminEmail,
////                Email = adminEmail,
////                FirstName = "System",
////                LastName = "Administrator",
////                EmailConfirmed = true
////            };

////            await userManager.CreateAsync(adminUser, "Admin@123");
////            await userManager.AddToRoleAsync(adminUser, "Admin");
////        }
////    }
////    catch (Exception ex)
////    {
////        Console.WriteLine($"Error creating admin user: {ex.Message}");
////    }
////});

//Task.Run(async () =>
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var services = scope.ServiceProvider;
//        try
//        {
//            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//            if (!await roleManager.RoleExistsAsync("Admin"))
//                await roleManager.CreateAsync(new IdentityRole("Admin"));

//            if (!await roleManager.RoleExistsAsync("User"))
//                await roleManager.CreateAsync(new IdentityRole("User"));

//            var adminEmail = "admin@inventory.com";
//            var adminUser = await userManager.FindByEmailAsync(adminEmail);
//            if (adminUser == null)
//            {
//                adminUser = new ApplicationUser
//                {
//                    UserName = adminEmail,
//                    Email = adminEmail,
//                    FirstName = "System",
//                    LastName = "Administrator",
//                    EmailConfirmed = true
//                };

//                await userManager.CreateAsync(adminUser, "Admin@123");
//                await userManager.AddToRoleAsync(adminUser, "Admin");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error creating admin user: {ex.Message}");
//        }
//    }
//});

//Console.WriteLine($"API running on http://localhost:5004");
//app.Run();

using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.Repositories;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// HARDCODE JWT SETTINGS FOR TESTING (Remove this after it works)
var jwtIssuer = "http://localhost:5004";
var jwtAudience = "http://localhost:4200";
var jwtSecret = "MyVeryLongSecretKeyThatIsAtLeast32CharactersLong123!";

Console.WriteLine($"=== JWT HARDCODED SETTINGS ===");
Console.WriteLine($"Issuer: {jwtIssuer}");
Console.WriteLine($"Audience: {jwtAudience}");
Console.WriteLine($"Secret: {jwtSecret}");
Console.WriteLine($"===============================");

// Configure JWT Authentication
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
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Message Received - Header: {authHeader}");
            if (!string.IsNullOrEmpty(authHeader))
            {
                var token = authHeader.Replace("Bearer ", "").Trim();
                context.Token = token;
                Console.WriteLine($"Token extracted successfully. Length: {token.Length}");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Auth Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully!");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Create default admin user
using (var scope = app.Services.CreateScope())
{
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
            Console.WriteLine("Admin user created");
        }
        else
        {
            Console.WriteLine("Admin user already exists");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

Console.WriteLine("Application started on http://localhost:5004");
app.Run();