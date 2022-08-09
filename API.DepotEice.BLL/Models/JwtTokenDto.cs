using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class JwtTokenDto
    {
        public string Issuer { get; }
        public string Audience { get; }
        public string Secret { get; }
        public int ExpirationInDays { get; }

        /// <summary>
        /// Generate an instance of <see cref="JwtTokenDto"/> with values from environment variables.
        /// If the variables are null, exceptions are thrown.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="FormatException"></exception>
        public JwtTokenDto()
        {
            Issuer = Environment.GetEnvironmentVariable("DEPOT_EICE_API_ISSUER") ??
                throw new NullReferenceException
                    (
                    "DEPOT_EICE_API_ISSUER environment variable does not exist!"
                    );

            Audience = Environment.GetEnvironmentVariable("DEPOT_EICE_API_AUDIENCE") ??
                throw new NullReferenceException
                    (
                    "DEPOT_EICE_API_AUDIENCE environment variable does not exist!"
                    );

            Secret = Environment.GetEnvironmentVariable("DEPOT_EICE_API_SECRET") ??
                throw new NullReferenceException
                    (
                    "DEPOT_EICE_API_SECRET environment variable does not exist!"
                    );

            string expirationDaysEnv =
                Environment.GetEnvironmentVariable("DEPOT_EICE_API_EXPIRATION_DAYS") ??
                    throw new NullReferenceException
                        (
                        "DEPOT_EICE_API_EXPIRATION_DAYS environment variable does not exist!"
                        );


            if (!int.TryParse(expirationDaysEnv, out int expirationInDays))
            {
                throw new FormatException
                    (
                    "DEPOT_EICE_API_EXPIRATION_DAYS is not an integer!"
                    );
            }

            ExpirationInDays = expirationInDays;
        }

        /// <summary>
        /// Create an instance of <see cref="JwtTokenDto"/> by initializing its properties. 
        /// Exceptions are thrown if one of the properties is null or if 
        /// <paramref name="expirationInDays"/> is smaller or equals to 0.
        /// </summary>
        /// <param name="issuer">
        /// The JWT Token Issuer
        /// </param>
        /// <param name="audience">
        /// The JWT Token Audience
        /// </param>
        /// <param name="secret">
        /// The JWT Token Secret
        /// </param>
        /// <param name="expirationInDays">
        /// The number of days in which the token will expires
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public JwtTokenDto(string issuer, string audience, string secret, int expirationInDays)
        {
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException(nameof(audience));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException(nameof(secret));
            }

            if (expirationInDays <= 0)
            {
                throw new IndexOutOfRangeException(nameof(expirationInDays));
            }

            Issuer = issuer;
            Audience = audience;
            Secret = secret;
            ExpirationInDays = expirationInDays;
        }
    }
}
