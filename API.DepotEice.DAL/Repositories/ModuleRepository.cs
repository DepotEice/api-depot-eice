﻿using API.DepotEice.DAL.Entities;
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
    /// Get all the users of the given module
    /// </summary>
    /// <param name="moduleId">The id of the module</param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="UserEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<UserEntity> GetModuleUsers(int moduleId)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        string query =
            @"SELECT u.* 
            FROM [dbo].[UserModules] um 
            INNER JOIN [dbo].[Users] u ON u.[Id] = um.[UserId] 
            WHERE [ModuleId] = @moduleId";

        Command command = new Command(query);

        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteReader(command, x => x.DbToUser());
    }

    /// <summary>
    /// Get all the users who have a specific role name for the given module
    /// </summary>
    /// <param name="moduleId">The id of the module</param>
    /// <param name="role">The name of the role</param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="UserEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
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
    /// Get all the users who have a specific role name and with the given status for the given module
    /// </summary>
    /// <param name="moduleId">The id of the module</param>
    /// <param name="role">The name of the role</param>
    /// <param name="isAccepted">Status of the users in the module</param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="UserEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IEnumerable<UserEntity> GetModuleUsers(int moduleId, string role, bool isAccepted)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        if (string.IsNullOrEmpty(role))
        {
            throw new ArgumentNullException(nameof(role));
        }

        string query =
            @"SELECT u.*
            FROM Modules AS m
            INNER JOIN UserModules AS um ON um.ModuleId = m.Id
            INNER JOIN Users AS u ON u.Id = um.UserId
            INNER JOIN UserRoles AS ur ON ur.UserId = u.Id
            INNER JOIN Roles AS r ON r.Id = ur.RoleId
            WHERE m.Id = @moduleId AND r.Name = @role AND um.IsAccepted = @isAccepted";

        Command command = new Command(query);

        command.AddParameter("moduleId", moduleId);
        command.AddParameter("role", role);
        command.AddParameter("isAccepted", isAccepted);

        return _connection.ExecuteReader(command, u => u.DbToUser());
    }

    /// <summary>
    /// Get the status of the requested user in the module
    /// </summary>
    /// <param name="moduleId">
    /// The id of the module
    /// </param>
    /// <param name="userId">
    /// The id of the suer
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool? GetUserModuleStatus(int moduleId, string userId)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        string query =
            @"SELECT um.IsAccepted
            FROM UserModules AS um
            WHERE um.UserId = @userId AND um.ModuleId = @moduleId";

        Command command = new Command(query);

        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteReader(command, u => (bool?)u["IsAccepted"]).SingleOrDefault();
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
        {
            throw new ArgumentNullException(nameof(userId));
        }

        string query =
            @"SELECT 
                m.* 
            FROM 
                [dbo].[UserModules] um 
            INNER JOIN 
                [dbo].[Modules] m 
            ON 
                m.Id = um.[ModuleId] 
            WHERE 
                um.[UserId] = @userId";

        Command command = new Command(query);

        command.AddParameter("userId", userId);

        return _connection.ExecuteReader(command, module => Mapper.DbToModule(module));
    }

    /// <summary>
    /// Get all <see cref="ModuleEntity"/> related to <paramref name="userId"/> and <paramref name="role"/>
    /// </summary>
    /// <param name="moduleId">
    /// The id of the module
    /// </param>
    /// <returns>
    /// <see cref="IEnumerable{T}"/> where T is <see cref="UserModuleEntity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IEnumerable<UserModuleEntity> GetUserModules(int moduleId)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        string query =
            @"SELECT u.*
            FROM UserModules AS um
            INNER JOIN Users AS u ON u.Id = um.UserId
            WHERE um.ModuleId = @moduleId";

        Command command = new Command(query);

        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteReader(command, u => u.DBToUserModule());
    }

    /// <summary>
    /// Add a user to a module. By default the user is not accepted
    /// </summary>
    /// <param name="userId">
    /// The id of the user
    /// </param>
    /// <param name="moduleId">
    /// The id of the module
    /// </param>
    /// <returns>
    /// <see cref="bool"/> true if the user has been added to the module, false otherwise
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool AddUserToModule(string userId, int moduleId)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        string query = "INSERT INTO [dbo].[UserModules] (UserId, ModuleId) VALUES (@userId, @moduleId)";

        Command command = new Command(query);

        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool SetUserStatus(string userId, int moduleId, bool isAccepted = true)
    {
        string query =
            @"UPDATE 
                [dbo].[UserModules] 
            SET 
                [IsAccepted] = @isAccepted 
            WHERE 
                [UserId] = @userId 
                AND 
                [ModuleId] = @moduleId";

        Command command = new Command(query);

        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);
        command.AddParameter("isAccepted", isAccepted);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// Delete a UserModules record from the database where the user id and module id match
    /// </summary>
    /// <param name="userId">
    /// The id of the user
    /// </param>
    /// <param name="moduleId">
    /// The id of the module
    /// </param>
    /// <returns>
    /// <c>true</c> If the record has been deleted, <c>false</c> otherwise
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool DeleteUserFromModule(string userId, int moduleId)
    {
        if (moduleId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moduleId));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        string query = "DELETE FROM [dbo].[UserModules] WHERE [UserId] = @userId AND [ModuleId] = @moduleId";

        Command command = new Command(query);

        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

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
    public ModuleEntity? GetByKey(int key)
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
}
