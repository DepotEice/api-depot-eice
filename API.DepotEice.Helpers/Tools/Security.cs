using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;

namespace API.DepotEice.Helpers.Tools
{
    public static class Security
    {
        public static string GenerateHMACSHA512(this string input, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(input, salt, KeyDerivationPrf.HMACSHA512, 100000, 512 / 8));
        }
    }
}
