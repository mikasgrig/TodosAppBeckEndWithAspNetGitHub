using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestAPI.Models;

namespace RestAPI.Services
{
    public interface IApikeyService
    {
        Task<ApiKeyModel> CreateApiKey(string username, string password);
        Task<IEnumerable<ApiKeyModel>> GetAllApiKey(string username, string password);
        Task<ApiKeyModel> UpdateApiKey(Guid id, bool state);
    }
}