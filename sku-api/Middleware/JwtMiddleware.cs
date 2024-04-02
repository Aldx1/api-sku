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

    /// <summary>
    /// Asynchronous method invoked by the middleware pipeline to handle each HTTP request.
    /// Extracts the JWT token from the Authorization header, validates it, and sets the user principal in the HttpContext.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault();

        if (!string.IsNullOrEmpty(token))
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

    /// <summary>
    /// Validates the JWT token 
    /// </summary>
    /// <param name="token"></param>
    /// <returns>A ClaimsPrincipal representing the user if the token is valid</returns>
    private ClaimsPrincipal ValidateToken(string token)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        return principal.Clone();
    }
}