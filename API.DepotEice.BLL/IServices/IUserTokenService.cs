using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IUserTokenService
    {
        UserTokenModel? CreateUserToken(UserTokenModel model);
        bool DeleteUserToken(string id);
        UserTokenModel? GetUserToken(string id);
        IEnumerable<UserTokenModel> GetUserTokens(string id);
    }
}
