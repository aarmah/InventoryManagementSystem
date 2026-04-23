using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Diagnostics;
using InventoryAPI.Models;
using InventoryAPI.DTOs;

//namespace InventoryAPI.Services
//{
//    public interface IAuthService
//    {
//        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
//        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
//    }

//    public class AuthService : IAuthService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<AuthService> _logger;

//        public AuthService(
//            UserManager<ApplicationUser> userManager,
//            RoleManager<IdentityRole> roleManager,
//            IConfiguration configuration,
//            ILogger<AuthService> logger)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _configuration = configuration;
//            _logger = logger;
//        }

//        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
//        {
//            var stopwatch = Stopwatch.StartNew();

//            try
//            {
//                _logger.LogInformation($"Login attempt for user: {loginDto.Email}");

//                // Step 1: Find user
//                var findUserStopwatch = Stopwatch.StartNew();
//                var user = await _userManager.FindByEmailAsync(loginDto.Email);
//                findUserStopwatch.Stop();
//                _logger.LogInformation($"Find user took: {findUserStopwatch.ElapsedMilliseconds}ms");

//                if (user == null)
//                {
//                    _logger.LogWarning($"User not found: {loginDto.Email}");
//                    throw new Exception("Invalid email or password!");
//                }

//                // Step 2: Check password
//                var checkPasswordStopwatch = Stopwatch.StartNew();
//                var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
//                checkPasswordStopwatch.Stop();
//                _logger.LogInformation($"Check password took: {checkPasswordStopwatch.ElapsedMilliseconds}ms");

//                if (!passwordValid)
//                {
//                    _logger.LogWarning($"Invalid password for user: {loginDto.Email}");
//                    throw new Exception("Invalid email or password!");
//                }

//                // Step 3: Get roles
//                var getRolesStopwatch = Stopwatch.StartNew();
//                var userRoles = await _userManager.GetRolesAsync(user);
//                getRolesStopwatch.Stop();
//                _logger.LogInformation($"Get roles took: {getRolesStopwatch.ElapsedMilliseconds}ms");

//                // Step 4: Generate token
//                var generateTokenStopwatch = Stopwatch.StartNew();
//                var token = await GenerateToken(user, userRoles);
//                generateTokenStopwatch.Stop();
//                _logger.LogInformation($"Generate token took: {generateTokenStopwatch.ElapsedMilliseconds}ms");

//                stopwatch.Stop();
//                _logger.LogInformation($"Total login time for {loginDto.Email}: {stopwatch.ElapsedMilliseconds}ms");

//                return token;
//            }
//            catch (Exception ex)
//            {
//                stopwatch.Stop();
//                _logger.LogError(ex, $"Error during login for user: {loginDto.Email} after {stopwatch.ElapsedMilliseconds}ms");
//                throw;
//            }
//        }

//        //private async Task<AuthResponseDto> GenerateToken(ApplicationUser user, IList<string> userRoles)
//        //{
//        //    var authClaims = new List<Claim>
//        //    {
//        //        new Claim(ClaimTypes.NameIdentifier, user.Id),
//        //        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
//        //        new Claim(ClaimTypes.GivenName, user.FirstName),
//        //        new Claim(ClaimTypes.Surname, user.LastName),
//        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        //    };

//        //    foreach (var userRole in userRoles)
//        //    {
//        //        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
//        //    }

//        //    var jwtSecret = _configuration["JWT:Secret"] ?? "DevelopmentSecretKey12345678901234567890";
//        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

//        //    var token = new JwtSecurityToken(
//        //        issuer: _configuration["JWT:ValidIssuer"] ?? "http://localhost:5004",
//        //        audience: _configuration["JWT:ValidAudience"] ?? "http://localhost:4200",
//        //        expires: DateTime.UtcNow.AddHours(8),
//        //        claims: authClaims,
//        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
//        //    );

//        //    return new AuthResponseDto
//        //    {
//        //        Token = new JwtSecurityTokenHandler().WriteToken(token),
//        //        Email = user.Email ?? string.Empty,
//        //        FirstName = user.FirstName,
//        //        LastName = user.LastName,
//        //        Role = userRoles.FirstOrDefault() ?? "User",
//        //        Expiration = token.ValidTo
//        //    };
//        //}

//        private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
//        {
//            var authClaims = new List<Claim>
//    {
//        new Claim(ClaimTypes.NameIdentifier, user.Id),
//        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
//        new Claim(ClaimTypes.GivenName, user.FirstName),
//        new Claim(ClaimTypes.Surname, user.LastName),
//        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//    };

//            var userRoles = await _userManager.GetRolesAsync(user);
//            foreach (var userRole in userRoles)
//            {
//                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
//            }

//            // Try multiple possible configuration keys
//            var secretKey = _configuration["JWT:Secret"] ?? _configuration["Jwt:SecretKey"] ?? "DefaultSecretKeyForDevelopmentPurposesOnly12345!";
//            var issuer = _configuration["JWT:ValidIssuer"] ?? _configuration["Jwt:ValidIssuer"] ?? "http://localhost:5004";
//            var audience = _configuration["JWT:ValidAudience"] ?? _configuration["Jwt:ValidAudience"] ?? "http://localhost:4200";

//            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

//            var token = new JwtSecurityToken(
//                issuer: issuer,
//                audience: audience,
//                expires: DateTime.UtcNow.AddDays(7),
//                claims: authClaims,
//                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
//            );

//            return new AuthResponseDto
//            {
//                Token = new JwtSecurityTokenHandler().WriteToken(token),
//                Email = user.Email ?? string.Empty,
//                FirstName = user.FirstName,
//                LastName = user.LastName,
//                Role = userRoles.FirstOrDefault() ?? "User",
//                Expiration = token.ValidTo
//            };
//        }

//        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
//        {
//            var stopwatch = Stopwatch.StartNew();

//            try
//            {
//                var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
//                if (userExists != null)
//                    throw new Exception("User with this email already exists!");

//                var user = new ApplicationUser
//                {
//                    Email = registerDto.Email,
//                    UserName = registerDto.Email,
//                    FirstName = registerDto.FirstName,
//                    LastName = registerDto.LastName,
//                    SecurityStamp = Guid.NewGuid().ToString(),
//                    CreatedAt = DateTime.UtcNow
//                };

//                var result = await _userManager.CreateAsync(user, registerDto.Password);
//                if (!result.Succeeded)
//                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

//                await _userManager.AddToRoleAsync(user, "User");

//                var userRoles = await _userManager.GetRolesAsync(user);
//                var token = await GenerateToken(user, userRoles);

//                stopwatch.Stop();
//                _logger.LogInformation($"Registration for {registerDto.Email} took: {stopwatch.ElapsedMilliseconds}ms");

//                return token;
//            }
//            catch (Exception ex)
//            {
//                stopwatch.Stop();
//                _logger.LogError(ex, $"Error during registration for {registerDto.Email} after {stopwatch.ElapsedMilliseconds}ms");
//                throw;
//            }
//        }
//    }
//}

namespace InventoryAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                throw new Exception("User with this email already exists!");

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");

            return await GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new Exception("Invalid email or password!");

            return await GenerateToken(user);
        }

        //private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
        //{
        //    var authClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id),
        //        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        //        new Claim(ClaimTypes.GivenName, user.FirstName),
        //        new Claim(ClaimTypes.Surname, user.LastName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    foreach (var userRole in userRoles)
        //    {
        //        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //    }

        //    // Get JWT settings with fallbacks
        //    var jwtSecret = _configuration["JWT:Secret"] ?? _configuration["Jwt:SecretKey"] ?? "DefaultSecretKeyForDevelopmentPurposesOnly12345!";
        //    var jwtIssuer = _configuration["JWT:ValidIssuer"] ?? _configuration["Jwt:ValidIssuer"] ?? "http://localhost:5004";
        //    var jwtAudience = _configuration["JWT:ValidAudience"] ?? _configuration["Jwt:ValidAudience"] ?? "http://localhost:4200";

        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        //    var token = new JwtSecurityToken(
        //        issuer: jwtIssuer,
        //        audience: jwtAudience,
        //        expires: DateTime.UtcNow.AddDays(7),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //    );

        //    return new AuthResponseDto
        //    {
        //        Token = new JwtSecurityTokenHandler().WriteToken(token),
        //        Email = user.Email ?? string.Empty,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Role = userRoles.FirstOrDefault() ?? "User",
        //        Expiration = token.ValidTo
        //    };
        //}
        private async Task<AuthResponseDto> GenerateToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                //new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                //new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(ClaimTypes.NameIdentifier, user.Id),
                //new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                //new Claim(ClaimTypes.GivenName, user.FirstName),
                //new Claim(ClaimTypes.Surname, user.LastName),

                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin") // Force add role claim
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Use the SAME hardcoded values as in Program.cs
            var jwtIssuer = "http://localhost:5004";
            var jwtAudience = "http://localhost:4200";
            var jwtSecret = "MyVeryLongSecretKeyThatIsAtLeast32CharactersLong123!";

            Console.WriteLine($"Generating token with Issuer: {jwtIssuer}, Audience: {jwtAudience}");

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: DateTime.UtcNow.AddDays(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            Console.WriteLine($"Token generated with Issuer: {jwtIssuer}, Audience: {jwtAudience}");

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = userRoles.FirstOrDefault() ?? "User",
                Expiration = token.ValidTo
            };
        }


    }
}