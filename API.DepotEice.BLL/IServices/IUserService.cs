using API.DepotEice.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.IServices
{
    public interface IUserService
    {
        bool ActivateUser(string id, bool isActive);
        UserModel? CreateUser(UserModel user);
        bool DeleteUser(string id);
        IEnumerable<UserModel> GetUsers();
        UserModel? GetUser(string id);
        bool UpdatePassword(string id, string oldPassword, string newPassword, string salt);
    }
}
