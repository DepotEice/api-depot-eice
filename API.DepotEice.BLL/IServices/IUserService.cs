using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices
{
    public interface IUserService
    {
        string LogIn(string email, string password, JwtTokenDto tokenDto);
        UserDto? CreateUser(UserDto user);
        bool DeleteUser(string id);
        IEnumerable<UserDto> GetUsers();
        UserDto? GetUser(string id);
        bool UpdatePassword(string id, string oldPassword, string newPassword, string salt);
        public bool UserExist(string id);
        public bool EmailExist(string email);
    }
}
