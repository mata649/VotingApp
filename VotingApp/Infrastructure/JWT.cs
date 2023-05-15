using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VotingApp.Rest;

public class JWT
{
    private IConfiguration _configuration { get; set; }

    public JWT(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Guid userID)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, userID.ToString()),

        };
        string? secretKey = _configuration.GetSection("JWT:Key").Value;

        if (secretKey is null) return string.Empty;

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512);
        JwtSecurityToken securityToken = new(
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);

    }

    public static Guid GetCurrentUserID(HttpContext context)
    {
        return Guid.Parse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "");
    }
}

