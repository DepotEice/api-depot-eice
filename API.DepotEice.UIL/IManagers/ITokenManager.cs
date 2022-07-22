using API.DepotEice.UIL.Models;

namespace API.DepotEice.UIL.IManagers
{
    public interface ITokenManager
    {
        string GenerateJWT(LoggedInUserModel model);
    }
}
