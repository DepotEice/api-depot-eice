using API.DepotEice.UIL.Interfaces;
using API.DepotEice.UIL.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.DepotEice.UIL.Managers;

/// <summary>
/// Handle everything around the JWT Token management. Creation and validation
/// </summary>
public class TokenManager : ITokenManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    /// <summary>
    /// Instanciate <see cref="TokenManager"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TokenManager(IConfiguration configuration, ILogger<TokenManager> logger)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _configuration = configuration;
        _logger = logger;
    }

    // TODO: Improve the documentation
    /// <summary>
    /// Create a JWT token
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public string GenerateJWT(LoggedInUserModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

#if DEBUG
        string? secretStr = _configuration["JWT:JWT_SECRET"];
#else
        string? secretStr = Environment.GetEnvironmentVariable("JWT_SECRET") ??
            throw new NullReferenceException($"There is no environment variable named JWT_SECRET");
#endif
        if (string.IsNullOrEmpty(secretStr))
        {
            _logger.LogError($"{DateTime.Now} - The JWT Secret string is null or empty");
            throw new NullReferenceException(nameof(secretStr));
        }

        byte[] secretArr = Encoding.ASCII.GetBytes(secretStr);

        SymmetricSecurityKey securityKey = new(secretArr);

        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha512);

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, model.Id.ToString()),
            new Claim(ClaimTypes.Email, model.Email)
        };

        if (model.Roles.Count() > 0)
        {
            claims.AddRange(model.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        }
#if DEBUG
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials,
            Audience = _configuration["JWT:JWT_AUDIENCE"] ??
                throw new NullReferenceException("There is no environment variable named JWT_AUDIENCE"),
            Issuer = _configuration["JWT:JWT_ISSUER"] ??
                throw new NullReferenceException("There is no environment variable named JWT_ISSUER")
        };
#else
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
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

    /// <summary>
    /// Validates the JWT token and verify if the user requesting the validation is the one owning the token
    /// </summary>
    /// <param name="userId">User requesting the validation</param>
    /// <param name="jwtToken">JWT Token</param>
    /// <returns>
    /// <c>true</c> If the token is valid. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ValidateJwtToken(string userId, string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
        {
            throw new ArgumentNullException(nameof(jwtToken));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
#if DEBUG
        byte[] key = Encoding.ASCII.GetBytes(_configuration["JWT:JWT_SECRET"]);
#else
        byte[] key = Encoding.ASCII.GetBytes(
            Environment.GetEnvironmentVariable("JWT_SECRET") ??
                throw new NullReferenceException($"There is no environment variable named JWT_SECRET")
            );
#endif

        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
        {
#if DEBUG
            ValidIssuer = _configuration["JWT:JWT_ISSUER"],
            ValidAudience = _configuration["JWT:JWT_AUDIENCE"],
#else
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                throw new NullReferenceException($"There is no environment variable named JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                throw new NullReferenceException($"There is no environment variable named JWT_AUDIENCE"),
#endif
            ValidateIssuer = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
        };

        try
        {
            ClaimsPrincipal validatedToken = tokenHandler
                .ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken securityToken);

            Claim? validatedTokenUserIdClaim = validatedToken.Claims.SingleOrDefault(c => c.Type.Equals(ClaimTypes.Sid));

            if (validatedTokenUserIdClaim is null)
            {
                return false;
            }

            string? validatedTokenUserId = validatedTokenUserIdClaim.Value;

            if (string.IsNullOrEmpty(validatedTokenUserId))
            {
                return false;
            }

            return validatedTokenUserId.Equals(userId);
        }
        catch (SecurityTokenException e)
        {
            _logger.LogInformation(
                "{date} - The token verification for token \"{jwtToken}\" failed\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                jwtToken,
                e.Message,
                e.StackTrace
            );

            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{date} - An exception occurred during \"{fn}\" :\n\"{eMsg}\"\n\"{eStr}\"",
                DateTime.Now,
                nameof(ValidateJwtToken),
                e.Message,
                e.StackTrace
            );

            return false;
        }
    }
}
