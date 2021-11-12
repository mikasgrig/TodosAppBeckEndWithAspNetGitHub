using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private const string TableName = "apikey";
        private readonly ISqlClient _sqlClient;
        public ApiKeyRepository(ISqlClient sqlClient)
        {
            _sqlClient = sqlClient;
        }
        public Task<IEnumerable<ApiKeyReadModel>> GetApiKeysAsync(Guid userId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE UserId = @UserId";
            
            return _sqlClient.QueryAsync<ApiKeyReadModel>(sql, new {UserId = userId});
        }

        public Task<ApiKeyReadModel> GetByApiKeysAsync(string apikey)
        {
            var sql = $"SELECT * FROM {TableName} WHERE ApiKey = @ApiKey";
            
            return _sqlClient.QuerySingleOrDefaultAsync<ApiKeyReadModel>(sql, new {ApiKey = apikey});
        }

        public Task<ApiKeyReadModel> GetByApiKeyIdAsync(Guid apiKeyId)
        {
            var sql = $"SELECT * FROM {TableName} WHERE Id = @Id";
            
            return _sqlClient.QuerySingleOrDefaultAsync<ApiKeyReadModel>(sql, new {Id = apiKeyId});
        }

        public Task<int> SaveAsync(ApiKeyReadModel apikey)
        {
            var sql = @$"INSERT INTO {TableName} (Id, ApiKey, UserId, IsActive, DateCreated, ExpirationDate) 
                        VALUES (@Id, @ApiKey, @UserId, @IsActive, @DateCreated, @ExpirationDate)";

            return _sqlClient.ExecuteAsync(sql, apikey);
        }

        public Task<int> UpdateIsActive(Guid id, bool isActive)
        {
            var sql = $"UPDATE {TableName} SET IsActive = @IsActive WHERE Id = @Id";
            return _sqlClient.ExecuteAsync(sql, new
            {
                Id = id,
                IsActive = isActive
            }
            );
        }
    }
}