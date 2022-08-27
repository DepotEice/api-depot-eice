using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IAuthRepository
{
    bool SingUp(UserEntity entity);
    UserEntity? SignIn(string email, string password);
}
