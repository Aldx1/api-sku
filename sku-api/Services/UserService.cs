using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class UserService : IUserService
{
    private readonly StoreDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Serilog.ILogger _logger;

    public UserService(StoreDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, Serilog.ILogger logger)
    {
        _context = context;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IResult> AuthenticateUser(LoginModel loginModel)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginModel.Username);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            if (!VerifyPassword(user, loginModel.Password))
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Results.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<IResult> CreateUser(LoginModel loginModel)
    {
        try
        {
            var userValid = NewUserValid(loginModel.Username, loginModel.Password);

            if (!userValid.Item1)
            {
                return Results.BadRequest(userValid.Item2);
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(loginModel.Password);

            var newUser = new User() { Username = loginModel.Username, PasswordHash = passwordHash };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(newUser);
            return Results.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.Error<Exception>(ex.Message, ex);
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return token;
    }

    private bool VerifyPassword(User user, string password)
    {
        if (user == null || user.PasswordHash == null)
        {
            return false;
        }

        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return isValid;
    }

    private (bool, string) NewUserValid(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Username cannot be empty");
        }

        if (username.Length <= 5)
        {
            return (false, "Username must be at least 6 chars");
        }

        if (string.IsNullOrEmpty(password))
        {
            return (false, "Password cannot be empty");
        }

        if (password.Length <= 5)
        {
            return (false, "Password must be at least 6 chars");
        }

        if (_context.Users.Any(u => u.Username == username))
        {
            return (false, "Username already exists");
        }

        return (true, "");
    }

    public int? GetUserId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            int userId;
            string userIdStr = context.User.Claims.EmptyIfNull().FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out userId))
            {
                return null;
            }
            return userId;
        }

        return null;
    }
}