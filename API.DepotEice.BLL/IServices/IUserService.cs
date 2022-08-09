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
        UserDto? CreateUser(UserDto user);
        bool DeleteUser(string id);
        IEnumerable<UserDto> GetUsers();
        UserDto? GetUser(string id);
        bool UpdatePassword(string id, string oldPassword, string newPassword, string salt);
        public bool UserExist(string id);
        public bool EmailExist(string email);
    }
}
