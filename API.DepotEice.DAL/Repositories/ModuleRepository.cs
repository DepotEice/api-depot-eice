using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ModuleRepository : RepositoryBase, IModuleRepository
{
    public ModuleRepository(IDevHopConnection connection) : base(connection) { }

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

        string query = "SELECT u.* FROM [dbo].[UsersModules] um INNER JOIN [dbo].[Users] u ON u.[Id] = um.[UserId] WHERE [ModuleId] = @moduleId";

        Command command = new Command(query);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteReader(command, x => x.DbToUser());
    }

    public IEnumerable<UserEntity> GetModuleUsers(int moduleId, string role)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        if (string.IsNullOrEmpty(role))
        {
            throw new ArgumentNullException(nameof(role));
        }

        string query = @"
            SELECT u.*
            FROM Modules AS m
            INNER JOIN UserModules AS um ON um.ModuleId = m.Id
            INNER JOIN Users AS u ON u.Id = um.UserId
            INNER JOIN UserRoles AS ur ON ur.UserId = u.Id
            INNER JOIN Roles AS r ON r.Id = ur.RoleId
            WHERE m.Id = @moduleId AND r.Name = @role
            ";

        Command command = new Command(query);

        command.AddParameter("moduleId", moduleId);
        command.AddParameter("role", role);

        return _connection.ExecuteReader(command, u => u.DbToUser());
    }

    /// <summary>
    /// Retrieve all <see cref="ModuleEntity"/> related to <paramref name="userId"/>
    /// </summary>
    /// <param name="userId">
    /// ID of the <see cref="UserEntity"/>
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of type <see cref="ModuleEntity"/>. If the query
    /// returns no data then an empty <see cref="IEnumerable{T}"/> is returned
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<ModuleEntity> GetUserModules(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "SELECT m.* FROM [dbo].[UserModules] um INNER JOIN [dbo].[Modules] m ON m.Id = um.[ModuleId] WHERE um.[UserId] = @userId";

        Command command = new Command(query);

        command.AddParameter("userId", userId);

        return _connection.ExecuteReader(command, module => Mapper.DbToModule(module));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool AddUserToModule(string userId, int moduleId)
    {
        if (moduleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moduleId));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "INSERT INTO [dbo].[UserModules] (UserId, ModuleId) VALUES (@userId, @moduleId)";
        Command command = new Command(query);

        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool AcceptUser(string userId, int moduleId, bool isAccepted = true)
    {
        string query = "UPDATE [dbo].[UserModules] SET [IsAccepted] = @isAccepted WHERE [UserId] = @userId AND [ModuleId] = @moduleId";
        Command command = new Command(query);
        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);
        command.AddParameter("isAccepted", isAccepted);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool DeleteUserFromModule(string userId, int moduleId)
    {
        if (moduleId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moduleId));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "DELETE FROM [dbo].[UsersModules] WHERE [UserId] = @userId AND [ModuleId] = @moduleId";
        Command command = new Command(query);
        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);
        return _connection.ExecuteNonQuery(command) > 0;
    }

    #region BASIC CRUD

    /// <inheritdoc/>
    public IEnumerable<ModuleEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[Modules]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, module => module.DbToModule());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public int Create(ModuleEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spModules_Create", true);

        command.AddParameter("name", entity.Name);
        command.AddParameter("description", entity.Description);

        string scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
        {
            throw new DatabaseScalarNullException(nameof(scalarResult));
        }

        return int.Parse(scalarResult);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ModuleEntity GetByKey(int key)
    {
        if (key <= 0)
            throw new ArgumentOutOfRangeException(nameof(key));

        string query = "SELECT * FROM [dbo].[Modules] WHERE [Modules].[Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, module => Mapper.DbToModule(module))
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(int key, ModuleEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [Modules] SET [Name] = @name, [Description] = @description WHERE [Modules].[Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);
        command.AddParameter("name", entity.Name);
        command.AddParameter("description", entity.Description);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(int key)
    {
        if (key <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        string query = "DELETE FROM [dbo].[Modules] WHERE [Modules].[Id] = @id";

        Command command = new Command(query);

        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    #endregion
}
