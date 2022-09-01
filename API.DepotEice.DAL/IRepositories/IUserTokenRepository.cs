using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IUserTokenRepository : IRepositoryBase<string, UserTokenEntity>
{
    IEnumerable<UserTokenEntity> GetUserTokens(string userId);
    bool ApproveToken(UserTokenEntity entity);
    bool VerifyUserToken(UserTokenEntity userToken);
}
