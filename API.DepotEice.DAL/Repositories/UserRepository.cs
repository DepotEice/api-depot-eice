using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(IDevHopConnection connection) : base(connection) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ActivateDeactivateUser(string id, bool isActive = true)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        string query = 
            "UPDATE [dbo].[Users] " +
            "SET [IsActive] = @isActive, [EmailConfirmed] = 1, [SecurityStamp] = NEWID() " +
            "WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", id);
        command.AddParameter("isActive", isActive);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public UserEntity? LogIn(string email, string password, string salt)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        string query =
            "SELECT * FROM [dbo].[Users] " +
            "WHERE [NormalizedEmail] = @normalizedEmail AND [PasswordHash] = [dbo].[fnHashPassword](@password, @salt)";

        Command command = new Command(query);
        command.AddParameter("normalizedEmail", email.ToUpper());
        command.AddParameter("password", password);
        command.AddParameter("salt", salt);

        return _connection
            .ExecuteReader(command, record => record.DbToUser())
            .SingleOrDefault();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<UserEntity> GetModuleUsers(int moduleId)
    {
        if (moduleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moduleId));

        string query = "SELECT u.* FROM [dbo].[UserModules] um INNER JOIN [dbo].[Users] u ON u.[Id] = um.UserId WHERE [ModuleId] = @moduleId";

        Command command = new Command(query);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteReader(command, user => user.DbToUser());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool UpdatePassword(string userId, string passwordHash)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException(nameof(passwordHash));

        string query = "UPDATE [dbo].[Users] SET [PasswordHash] = @passwordHash, [SecurityStamp] = NEWID() WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", userId);
        command.AddParameter("passwordHash", passwordHash);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public string GetHashPwdFromEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        string query = "SELECT [PasswordHash] FROM [dbo].[Users] WHERE [NormalizedEmail] = @normalizedEmail";
        Command command = new Command(query);
        command.AddParameter("normalizedEmail", email.ToUpper());
        return _connection.ExecuteScalar(command).ToString(); ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public UserEntity GetUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentNullException(nameof(email));

        string query = "SELECT * FROM [dbo].[Users] WHERE [NormalizedEmail] = @normalizedEmail";

        Command command = new Command(query);
        command.AddParameter("normalizedEmail", email.ToUpper());

        return _connection
            .ExecuteReader(command, user => user.DbToUser())
            .SingleOrDefault();
    }

    public IEnumerable<UserEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[Users]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, user => user.DbToUser());
    }

    /// <summary>
    /// Creates a user in the database. The password hashing happens in the database with the help
    /// of the salt
    /// </summary>
    /// <param name="entity">The <see cref="UserEntity"/> to create</param>
    /// <param name="password">User's password that is going to be hashed</param>
    /// <param name="salt">The application salt used to hash the password</param>
    /// <returns>The newly created ID of the user if it went well</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public string Create(UserEntity entity, string password, string salt)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spUsers_Create", true);
        command.AddParameter("email", entity.Email);
        command.AddParameter("password", password);
        command.AddParameter("salt", salt);
        command.AddParameter("firstname", entity.FirstName);
        command.AddParameter("lastname", entity.LastName);
        command.AddParameter("birthdate", entity.BirthDate);

        string scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
            throw new DatabaseScalarNullException(nameof(scalarResult));

        return scalarResult;
    }

    /// <summary>
    /// Not implemented
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public string Create(UserEntity entity)
    {
        throw new NotImplementedException();
    }

    public UserEntity GetByKey(string key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        string query = "SELECT * FROM [dbo].[Users] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, user => user.DbToUser())
            .SingleOrDefault();
    }

    public bool Update(string key, UserEntity entity)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spUsers_UpdateInformations", true);

        command.AddParameter("id", entity.Id);
        command.AddParameter("firstName", entity.FirstName);
        command.AddParameter("lastName", entity.LastName);
        command.AddParameter("birthDate", entity.BirthDate);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool Delete(string key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        string query = "DELETE FROM [dbo].[Users] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
