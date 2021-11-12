using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Persistence.Models.ReadModels;

namespace Persistence.Repositories
{
    public interface IApiKeyRepository
    {
        Task<IEnumerable<ApiKeyReadModel>> GetApiKeysAsync(Guid userId);
        Task<ApiKeyReadModel> GetByApiKeysAsync(string apikey);
        Task<ApiKeyReadModel> GetByApiKeyIdAsync(Guid apiKeyId);
        Task<int> SaveAsync(ApiKeyReadModel user);
        Task<int> UpdateIsActive(Guid id, bool isActive);
    }
    
}