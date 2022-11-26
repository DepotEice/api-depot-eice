using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class UserTokenRepository : RepositoryBase, IUserTokenRepository
{
    public UserTokenRepository(IDevHopConnection connection) : base(connection) { }

    public IEnumerable<UserTokenEntity> GetUserTokens(string userId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "SELECT * FROM [dbo].[UserTokens] WHERE [UserId] = @userId";

        Command command = new Command(query);
        command.AddParameter("userId", userId);

        return _connection
            .ExecuteReader(command, userToken => userToken.DbToUserToken());
    }

    public bool VerifyUserToken(UserTokenEntity userToken)
    {
        if (userToken is null)
            throw new ArgumentNullException(nameof(userToken));

        return ApproveToken(userToken);
    }

    public bool ApproveToken(UserTokenEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spUserTokens_Approve", true);

        command.AddParameter("id", entity.Id);
        command.AddParameter("type", entity.Type);

        return (int)_connection.ExecuteScalar(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<UserTokenEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[UserTokens]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, ut => ut.DbToUserToken());
    }

    // TODO : Override the method and remove the UserSecurityStamp parameter
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public string Create(UserTokenEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spUserTokens_Create", true);
        command.AddParameter("type", entity.Type);
        command.AddParameter("expirationDate", entity.ExpirationDate);
        command.AddParameter("userId", entity.UserId);
        command.AddParameter("userSecurityStamp", entity.UserSecurityStamp);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return scalarResult;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public UserTokenEntity GetByKey(string key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        string query = "SELECT * FROM [dbo].[UserTokens] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, userToken => userToken.DbToUserToken())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException"></exception>
    public bool Update(string key, UserTokenEntity entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(string key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        string query = "DELETE FROM [dbo].[UserTokens] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
