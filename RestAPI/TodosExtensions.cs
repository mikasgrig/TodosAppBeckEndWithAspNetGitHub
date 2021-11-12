using Contracts.Models.ResponseModels;
using Persistence.Models.ReadModels;
using RestAPI.Models;

namespace RestAPI
{
    public static class TodosExtensions
    {
        public static TodosItemResponse MapToTodoItemResponse(this TodoItemReadModel model)
        {
            return new TodosItemResponse
            {
                Id = model.Id,
                UserId = model.UserId,
                Title = model.Title,
                Description = model.Description,
                Difficulty = model.Difficulty,
                IsDone = model.IsDone,
                DateCreated = model.DateCreated
            };
        }
        public static SignUpResponse MapToUserResponse(this UserReadModel user)
        {

            return new SignUpResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                DateCreDate = user.DateCreated
            };
        }
        public static ApiKeyResponse MapToApiKeyResponse(this ApiKeyModel apikey)
        {

            return new ApiKeyResponse
            {
                Id = apikey.Id,
                ApiKey = apikey.ApiKey,
                UserId = apikey.UserId,
                IsActive = apikey.IsActive,
                DateCreated = apikey.DateCreated,
                ExpirationDate = apikey.ExpirationDate
                
            };
        }

        public static ApiKeyModel MapToApiKey(this ApiKeyReadModel apikey)
        {

            return new ApiKeyModel
            {
                Id = apikey.Id,
                ApiKey = apikey.ApiKey,
                UserId = apikey.UserId,
                IsActive = apikey.IsActive,
                DateCreated = apikey.DateCreated,
                ExpirationDate = apikey.ExpirationDate

            };
        }
    }
}