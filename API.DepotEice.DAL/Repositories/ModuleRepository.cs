﻿using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using API.DepotEice.Helpers.Exceptions;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly IDevHopConnection _connection;

    /// <summary>
    /// Instanciate a <see cref="ModuleRepository"/>
    /// </summary>
    /// <param name="mapper">Automapper</param>
    /// <param name="connection">Database connection</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ModuleRepository(IDevHopConnection connection)
    {
        if (connection is null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        _connection = connection;
    }

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

        Command command = new Command("spCreateModule", true);

        command.AddParameter("name", entity.Name);
        command.AddParameter("description", entity.Description);

        string? scalarResult = _connection.ExecuteScalar(command).ToString();

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
        {
            throw new ArgumentOutOfRangeException(nameof(key));
        }

        Command command = new Command("spGetModule", true);

        command.AddParameter("id", key);

        return _connection
            .ExecuteReader(command, module => Mapper.DbToModule(module))
            .SingleOrDefault();
    }

    /// <summary>
    /// Update the given <paramref name="entity"/> related record in the database
    /// </summary>
    /// <param name="entity">
    /// <see cref="ModuleEntity"/> object to update
    /// </param>
    /// <returns>
    /// <c>true</c> If the Module has been updated. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Update(ModuleEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spUpdateModule", true);

        command.AddParameter("id", entity.Id);
        command.AddParameter("name", entity.Name);
        command.AddParameter("description", entity.Description);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public bool Delete(ModuleEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        Command command = new Command("spDeleteModule", true);

        command.AddParameter("id", entity.Id);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    /// <summary>
    /// Add a <see cref="UserEntity"/> to a <see cref="ModuleEntity"/>
    /// </summary>
    /// <param name="id">
    /// The ID of the <see cref="ModuleEntity"/>
    /// </param>
    /// <param name="userId">
    /// The ID of the <see cref="UserEntity"/> to add to the <see cref="ModuleEntity"/>
    /// </param>
    /// <returns>
    /// <c>true</c> If the user was correctly added to the module. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool AddUser(int id, string userId, bool isAccepted)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        Command command = new Command("spAddUserToModule", true);

        command.AddParameter("moduleId", id);
        command.AddParameter("userId", userId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    // TODO - Implémenter GET Module Students (spModuleStudentsGet)
    // TODO - Create an entity UserModule ?
    // TODO - Comment this function.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    public IEnumerable<UserEntity> GetModuleStudents(int moduleId)
    {
        Command command = new Command("spModuleStudentsGet", true);
        command.AddParameter("moduleId", moduleId);
        return _connection.ExecuteReader(command, x => x.DbToUser());
    }

    public bool DeleteStudentFromModule(string userId, int moduleId)
    {
        string query = "DELETE FROM [dbo].[UsersModules] WHERE [UserId] = @userId AND [ModuleId] = @moduleId";
        Command command = new Command(query);
        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);
        return _connection.ExecuteNonQuery(command) > 0;
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

        Command command = new Command("spGetUserModules", true);

        command.AddParameter("userId", userId);

        return _connection
            .ExecuteReader(command, module => Mapper.DbToModule(module));
    }

    /// <summary>
    /// Remove a <see cref="UserEntity"/> from a <see cref="ModuleEntity"/>
    /// </summary>
    /// <param name="id">
    /// <see cref="ModuleEntity"/>'s ID
    /// </param>
    /// <param name="userId">
    /// <see cref="UserEntity"/>'s ID
    /// </param>
    /// <returns>
    /// <c>true</c> If one or more rows were affected by the query. <c>false</c> Otherwise
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public bool RemoveUser(int id, string userId)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        Command command = new Command("spDeleteUserModule", true);

        command.AddParameter("moduleId", id);
        command.AddParameter("user_id", userId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool StuddentApply(string studentId, int moduleId)
    {
        Command command = new Command("spUserModulesApply", true);

        command.AddParameter("userId", studentId);
        command.AddParameter("moduleId", moduleId);

        return _connection.ExecuteNonQuery(command) > 0;
    }

    public bool StudentAcceptExempt(string userId, int moduleId, bool decision)
    {
        string query = "";
        Command command = new Command(query);
        command.AddParameter("userId", userId);
        command.AddParameter("moduleId", moduleId);
        command.AddParameter("decision", decision);

        return _connection.ExecuteNonQuery(command) > 0;
    }
}
