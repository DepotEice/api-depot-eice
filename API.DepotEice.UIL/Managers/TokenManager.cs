﻿using API.DepotEice.UIL.IManagers;
using API.DepotEice.UIL.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.DepotEice.UIL.Managers
{
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

            var claims = new[]
            {
                new Claim(ClaimTypes.Sid, model.Id.ToString()),
                new Claim(ClaimTypes.Email, model.Email),
                // TODO - Le reste des informations de l'utilisateur
            };

            // generate token that is valid for 7 days
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials,
                Audience = _builder.Configuration["AppSettings:Audience"],
                Issuer = _builder.Configuration["AppSettings:Issuer"]
            };

            JwtSecurityTokenHandler tokenHandler = new();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
