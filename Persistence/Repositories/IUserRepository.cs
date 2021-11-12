using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns></returns>
        Task<UserReadModel> GetAsync(string userName);
        /// <summary>
        /// Get user by username and password
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">User password</param>
        /// <returns></returns>
        Task<UserReadModel> GetAsync(string userName, string password);
        Task<int> SaveUserAsync(UserReadModel user);
    }
}