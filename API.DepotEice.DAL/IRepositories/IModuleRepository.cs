using API.DepotEice.DAL.Entities;

namespace API.DepotEice.DAL.IRepositories;

public interface IModuleRepository : IRepositoryBase<int, ModuleEntity>
{
    IEnumerable<UserEntity> GetModuleUsers(int moduleId);
    IEnumerable<UserEntity> GetModuleUsers(int moduleId, string role);
    IEnumerable<UserEntity> GetModuleUsers(int moduleId, string role, bool isAccepted);
    IEnumerable<ModuleEntity> GetUserModules(string userId);

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
    bool AddUserToModule(string studentId, int moduleId);
    bool SetUserStatus(string userId, int moduleId, bool decision);

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
    bool DeleteUserFromModule(string userId, int moduleId);

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
    bool? GetUserModuleStatus(int moduleId, string userId);
}
