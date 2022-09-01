using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.BLL.IServices;

public interface IAuthService
{
    UserDto? SignIn(string email, string password, string salt);
    bool SingUp(UserDto dto, string salt);
}
