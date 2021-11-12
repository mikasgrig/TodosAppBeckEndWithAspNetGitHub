using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Persistence.Models.ReadModels;
using Persistence.Repositories;
using RestAPI.Models;
using RestAPI.Options;

namespace RestAPI.Services
{
    public class ApikeyService : IApikeyService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IOptions<ApiKeySettings> _apiKeySettings;

        private ApiKeySettings ApiKeySettings => _apiKeySettings.Value;
        
        public ApikeyService(IUserRepository userRepository, IApiKeyRepository apiKeyRepository, IOptions<ApiKeySettings> apiKeySettings)
        {
            _userRepository = userRepository;
            _apiKeyRepository = apiKeyRepository;
            _apiKeySettings = apiKeySettings;
        }


        public async Task<ApiKeyModel> CreateApiKey(string username, string password)
        {
            var user = await _userRepository.GetAsync(username);
            if (user is null)
            {
                throw new BadHttpRequestException($"User with Username: {username} does not exists!", statusCode:404);
            }

            if (!user.Password.Equals((password)))
            {
                throw new BadHttpRequestException($"Wrong password for user: {user.UserName}", statusCode:400);
            }
            var allKeys = await _apiKeyRepository.GetApiKeysAsync(user.Id);
            if (ApiKeySettings.ApiKeyLimit < allKeys.Count() + 1)
            {
                throw new BadHttpRequestException($"Api key limit is reached", 400);
            }
            var apiKey = new ApiKeyReadModel
            {
                Id = Guid.NewGuid(),
                ApiKey = Guid.NewGuid().ToString("N"),
                UserId = user.Id,
                IsActive = true,
                DateCreated = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMinutes(ApiKeySettings.ExpirationTimeInMinutes)
            };

            await _apiKeyRepository.SaveAsync(apiKey);

            return new ApiKeyModel
            {
                Id = apiKey.Id,
                ApiKey = apiKey.ApiKey,
                UserId = apiKey.UserId,
                IsActive = apiKey.IsActive,
                DateCreated = apiKey.DateCreated,
                ExpirationDate = apiKey.ExpirationDate
            };
        }

        public async Task<IEnumerable<ApiKeyModel>> GetAllApiKey(string username, string password)
        {
            var user = await _userRepository.GetAsync(username);
            if (user is null)
            {
                throw new BadHttpRequestException($"User with Username: {username} does not exists!", statusCode:404 );
            }

            if (!user.Password.Equals((password)))
            {
                throw new BadHttpRequestException($"Wrong password for user: {user.UserName}", statusCode:400);
            }
            var apiKey = await _apiKeyRepository.GetApiKeysAsync(user.Id);
            return apiKey.Select(apikey => new ApiKeyModel
            {
                Id = apikey.Id,
                ApiKey = apikey.ApiKey,
                UserId = apikey.UserId,
                IsActive = apikey.IsActive,
                DateCreated = apikey.DateCreated,
                ExpirationDate = apikey.ExpirationDate
            });
        }

        public async Task<ApiKeyModel> UpdateApiKey(Guid id, bool state)
        {
            var apiKey = await _apiKeyRepository.GetByApiKeyIdAsync(id);

            if (apiKey is null)
            {
                throw new BadHttpRequestException($"Api key with Id: {id} does not exists", statusCode:404);
            }

            await _apiKeyRepository.UpdateIsActive(id, state);
            
            var updateapiKey = await _apiKeyRepository.GetByApiKeyIdAsync(id);
            return updateapiKey.MapToApiKey();
        }
    }
}