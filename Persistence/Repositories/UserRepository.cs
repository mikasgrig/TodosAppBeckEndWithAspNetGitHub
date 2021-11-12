using System;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng.Drbg;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string TableName = "users";
        private readonly ISqlClient _sqlClient;
        public UserRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public Task<UserReadModel> GetAsync(string userName)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserName = @UserName";
            
            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new {UserName = userName});
        }
        public Task<UserReadModel> GetAsync(string userName, string password)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserName = @UserName AND Password = @Password";
            
            return _sqlClient.QuerySingleOrDefaultAsync<UserReadModel>(sql, new
            {
                UserName = userName,
                Password = password
            });
        }

        public Task<int> SaveUserAsync(UserReadModel user)
        {
            var sql = @$"INSERT INTO {TableName} (Id, UserName, Password, DateCreated) 
                        VALUES (@Id, @UserName, @Password, @DateCreated)";

            return _sqlClient.ExecuteAsync(sql, user);
        }
    }
}