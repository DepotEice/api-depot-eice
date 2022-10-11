﻿using API.DepotEice.DAL.Entities;
using System.Runtime.CompilerServices;

namespace API.DepotEice.DAL.IRepositories;

public interface IUserRepository : IRepositoryBase<string, UserEntity>
{
    bool ActivateDeactivateUser(string id, bool isActive = true);
    string GetHashPwdFromEmail(string email);
    UserEntity GetUserByEmail(string email);
    IEnumerable<UserEntity> GetModuleUsers(int moduleId);
    UserEntity LogIn(string email, string passwordHash);
    bool UpdatePassword(string userId, string passwordHash);

    string Create(UserEntity entity, string password, string salt);
}