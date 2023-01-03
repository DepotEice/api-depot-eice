using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace API.DepotEice.UIL.Managers;

public class TokenManager : ITokenManager
{
    private readonly WebApplicationBuilder _builder;

    public TokenManager(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public string GenerateJWT(LoggedInUserModel model)
    {
        ArgumentNullException.ThrowIfNull(model.Email);

#if DEBUG
        string? secretStr = _builder.Configuration["JWT:JWT_SECRET"];
#else
        string? secretStr = Environment.GetEnvironmentVariable("JWT_SECRET") ??
            throw new NullReferenceException($"There is no environment variable named JWT_SECRET");
#endif
        byte[] secretArr = Encoding.ASCII.GetBytes(secretStr);

        SymmetricSecurityKey securityKey = new(secretArr);

        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512);

        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Sid, model.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Email, model.Email));

        if (model.Roles.Count() > 0)
        {
            claims.AddRange(model.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        }
#if DEBUG
        // generate token that is valid for 7 days
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
            Audience = _builder.Configuration["JWT:JWT_AUDIENCE"],
            Issuer = _builder.Configuration["JWT:JWT_ISSUER"]
        };
#else
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
            Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                throw new NullReferenceException("There is no environment variable named JWT_AUDIENCE"),
            Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                throw new NullReferenceException("There is no environment variable named JWT_ISSUER")
        };
#endif
        JwtSecurityTokenHandler tokenHandler = new();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
