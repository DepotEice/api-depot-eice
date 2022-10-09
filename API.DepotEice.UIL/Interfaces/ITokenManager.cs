using API.DepotEice.UIL.Models;

namespace API.DepotEice.UIL.Interfaces;

public interface ITokenManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    string GenerateJWT(LoggedInUserModel model);
}
