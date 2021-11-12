using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace RestAPI.Attributes
{
    [ApiKey]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var todoid = (Guid) context.ActionArguments["Id"];
            var apikey = context.HttpContext.Request.Headers["ApiKeyModel"].SingleOrDefault();

            if (string.IsNullOrWhiteSpace(apikey))
            {
                context.Result = new BadRequestObjectResult("ApiKeyModel header is missing");
                return;
            }

            var apiKeyRepository = context.HttpContext.RequestServices.GetService<IApiKeyRepository>();
            var key = await apiKeyRepository.GetByApiKeysAsync(apikey);
            if (key is null)
            {
                context.Result = new NotFoundObjectResult("ApiKeyModel is not found");
                return;
            }
            if (!key.IsActive)
            {
                context.Result = new BadRequestObjectResult("ApiKeyModel expired");
                return;
            }
            
            if (key.ExpirationDate <= DateTime.Now)
            {
                context.Result = new BadRequestObjectResult("ApiKeyModel expired");
                return;
            }
            context.HttpContext.Items.Add("userId", key.UserId);
            await next();
        }
    }
}