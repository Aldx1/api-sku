using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault();

        if (token != null)
        {
            try
            {
                var principal = ValidateToken(token);
                context.Items["User"] = principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating token: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await _next(context);
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };
        SecurityToken securityToken;
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        return principal.Clone();
    }
}