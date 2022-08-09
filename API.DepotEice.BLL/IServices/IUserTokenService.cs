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
        UserTokenDto? CreateUserToken(UserTokenDto model);
        bool DeleteUserToken(string id);
        UserTokenDto? GetUserToken(string tokenType, DateTime deliveryDate, string userId);
        IEnumerable<UserTokenDto> GetUserTokens(string id);
    }
}
