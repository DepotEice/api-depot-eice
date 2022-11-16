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

        string? secretStr = _builder.Configuration["AppSettings:Secret"];
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

        // generate token that is valid for 7 days
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = credentials,
            Audience = _builder.Configuration["AppSettings:Audience"],
            Issuer = _builder.Configuration["AppSettings:Issuer"]
        };

        JwtSecurityTokenHandler tokenHandler = new();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
