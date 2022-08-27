using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class AuthRepository : RepositoryBase, IAuthRepository
{
    public AuthRepository(IDevHopConnection connection) : base(connection) { }

    public UserEntity? SignIn(string email, string password)
    {
        throw new NotImplementedException();
    }

    public bool SingUp(UserEntity entity)
    {

        throw new NotImplementedException();
    }
}
