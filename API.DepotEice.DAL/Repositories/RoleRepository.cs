using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

/// <summary>
/// Repository for <see cref="RoleEntity"/>
/// </summary>
public class RoleRepository : RepositoryBase, IRoleRepository
{
    public RoleRepository(IDevHopConnection connection) : base(connection) { }

    /// <summary>
    /// Retrieve all <see cref="RoleEntity"/> related to a <see cref="UserEntity"/>
    /// </summary>
    /// <param name="userId">
    /// <see cref="UserEntity"/>'s ID
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="RoleEntity"/>. If no data is retrieved,
    /// returns an empty <see cref="IEnumerable{T}"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<RoleEntity> GetUserRoles(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = @"
            SELECT r.* 
            FROM [dbo].[UserRoles] ur 
            INNER JOIN [dbo].[Roles] r ON r.Id = ur.[RoleId] 
            WHERE ur.[UserId] = @userId";

        Command command = new Command(query);
        command.AddParameter("userId", userId);

        return _connection.ExecuteReader(command, role => role.DbToRole());
    }

    /// <summary>
    /// Add a <see cref="UserEntity"/> to a <see cref="RoleEntity"/>
    /// </summary>
    /// <param name="id">
    /// <see cref="RoleEntity"/>'s ID
    /// </param>
    /// <param name="userId">
    /// <see cref="UserEntity"/>'s ID
    /// </param>
    /// <returns>
    /// <c>true</c> If <see cref="UserEntity"/> has successfully been added to the 
    /// <see cref="RoleEntity"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool AddUser(string id, string userId)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "INSERT INTO [dbo].[UserRoles] ([UserId], [RoleId]) VALUES (@userid, @roleId)";

        Command command = new Command(query);
        command.AddParameter("roleId", id);
        command.AddParameter("userId", userId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// Remove a <see cref="RoleEntity"/> from <see cref="UserEntity"/>
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool RemoveUser(string roleId, string userId)
    {
        if (string.IsNullOrEmpty(roleId))
            throw new ArgumentNullException(nameof(roleId));

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        string query = "DELETE FROM [dbo].[UserRoles] WHERE ([UserId] = @userId) AND ([RoleId] = @roleId)";

        Command command = new Command(query);
        command.AddParameter("roleId", roleId);
        command.AddParameter("userId", userId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public RoleEntity GetByName(string name)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        string query = "SELECT * FROM [dbo].[Roles] WHERE [Name] = @name";

        Command command = new Command(query);
        command.AddParameter("name", name);

        return _connection
            .ExecuteReader(command, record => record.DbToRole())
            .SingleOrDefault();
    }

    #region BASIC CRUD

    /// <inheritdoc/>
    public IEnumerable<RoleEntity> GetAll()
    {
        string query = "SELECT * FROM [dbo].[Roles]";

        Command command = new Command(query);

        return _connection.ExecuteReader(command, role => role.DbToRole());
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DatabaseScalarNullException"></exception>
    public string Create(RoleEntity entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        Command command = new Command("spCreateRole", true);
        command.AddParameter("name", entity.Name);

        string scalarResult = _connection.ExecuteScalar(command).ToString();

        if (string.IsNullOrEmpty(scalarResult))
            throw new DatabaseScalarNullException(nameof(scalarResult));

        return scalarResult;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public RoleEntity? GetByKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        string query = "SELECT * FROM [dbo].[Roles] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, role => role.DbToRole())
            .SingleOrDefault();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(string key, RoleEntity entity)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        string query = "UPDATE [dbo].[Roles] SET [Name] = @name WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);
        command.AddParameter("name", entity.Name);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        string query = "DELETE FROM [dbo].[Roles] WHERE [Id] = @id";

        Command command = new Command(query);
        command.AddParameter("id", key);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    #endregion
}
