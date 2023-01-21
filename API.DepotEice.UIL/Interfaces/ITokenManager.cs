using API.DepotEice.UIL.Models;

namespace API.DepotEice.UIL.Interfaces;

/// <summary>
/// Token Manager interface
/// </summary>
public interface ITokenManager
{
    /// <summary>
    /// Create a JWT token
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    string GenerateJWT(LoggedInUserModel model);

    /// <summary>
    /// Validates the JWT token and verify if the user requesting the validation is the one owning the token
    /// </summary>
    /// <param name="userId">User requesting the validation</param>
    /// <param name="jwtToken">JWT Token</param>
    /// <returns>
    /// <c>true</c> If the token is valid. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    bool ValidateJwtToken(string userId, string jwtToken);
}
