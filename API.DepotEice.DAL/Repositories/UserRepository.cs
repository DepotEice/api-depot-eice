using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDevHopConnection _connection;

    public UserRepository(IDevHopConnection connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        _connection = connection;
    }

    /// <summary>
    /// Activate or deactivate <see cref="UserEntity"/> by settings its IsActive property
    /// </summary>
    /// <param name="id">
    /// ID of the <see cref="UserEntity"/> to activate/deactivate
    /// </param>
    /// <param name="isActive">
    /// <see cref="UserEntity"/>'s activation tag
    /// </param>
    /// <returns>
    /// <c>true</c> If the record has correctly been modified. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ActivateUser(string id, bool isActive)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        Command command = new Command("spActivateUser", true);

        command.AddParameter("id", id);
        command.AddParameter("isActive", isActive);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    // TODO : Create sp in database project
    public UserEntity? LogIn(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        Command command = new Command("spLogIn", true);

        command.AddParameter("email", email);
        command.AddParameter("password", password);

        return _connection.ExecuteReader(command, u => u.DbToUser())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public string Create(UserEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spCreateUser", true);

        command.AddParameter("email", entity.Email);
        command.AddParameter("password", entity.Password);
        command.AddParameter("salt", entity.Salt);
        command.AddParameter("firstName", entity.FirstName);
        command.AddParameter("lastName", entity.LastName);
        command.AddParameter("profilePicture", entity.ProfilePicture);
        command.AddParameter("birthdate", entity.BirthDate);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return scalarResult;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(UserEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spDeleteUser", true);

        command.AddParameter("id", entity.Id);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public IEnumerable<UserEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[Users]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, user => user.DbToUser());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public UserEntity? GetByKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        Command command = new Command("spGetUser", true);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, user => user.DbToUser())
            .SingleOrDefault();
    }

    // TODO : Verify if the spGetModuleUsers is implemented
    /// <summary>
    /// Retrieve all users linked to a module
    /// </summary>
    /// <param name="id">
    /// Module ID
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="UserEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<UserEntity> GetModuleUsers(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        Command command = new Command("spGetModuleUsers", true);

        command.AddParameter("moduleId", id);

        return _connection.ExecuteReader(command, user => user.DbToUser());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(UserEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spUpdateUserInformations", true);

        command.AddParameter("id", entity.Id);
        command.AddParameter("firstName", entity.FirstName);
        command.AddParameter("lastName", entity.LastName);
        command.AddParameter("profilePicture", entity.ProfilePicture);
        command.AddParameter("birthDate", entity.BirthDate);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// Update a <see cref="UserEntity"/> record's <see cref="UserEntity.Password"/> 
    /// property
    /// </summary>
    /// <param name="id">
    /// The ID of the <see cref="UserEntity"/> record to update
    /// </param>
    /// <param name="oldPassword">
    /// The old password of the <see cref="UserEntity"/>. If this parameter doesn't match the
    /// user's actual password, the method returns <c>false</c>
    /// </param>
    /// <param name="newPassword">
    /// The new password
    /// </param>
    /// <param name="salt">
    /// Application's salt
    /// </param>
    /// <returns>
    /// <c>true</c> If the password update has successfully been executed. <c>false</c> 
    /// Otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool UpdatePassword(string id, string oldPassword, string newPassword, string salt)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        if (string.IsNullOrEmpty(oldPassword))
        {
            throw new ArgumentNullException(nameof(oldPassword));
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            throw new ArgumentNullException(nameof(newPassword));
        }

        if (string.IsNullOrEmpty(salt))
        {
            throw new ArgumentNullException(nameof(salt));
        }

        Command command = new Command("spUpdateUserPassword", true);

        command.AddParameter("id", id);
        command.AddParameter("oldPassword", oldPassword);
        command.AddParameter("newPassword", newPassword);
        command.AddParameter("salt", salt);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
