using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.RequestModels;
using Contracts.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("apiKay")]
    public class ApiKeysController : ControllerBase
    {
        private readonly IApikeyService _apikeyService;

        public ApiKeysController(IApikeyService apikeyService)
        {
            _apikeyService = apikeyService;
        }


        [HttpPost]
        public async Task<ActionResult<ApiKeyResponse>> Create(ApiKeyRequest request)
        {
            try
            {
                var apiKey = await _apikeyService.CreateApiKey(request.UserName, request.Password);
                return apiKey.MapToApiKeyResponse();
            }
            catch (BadHttpRequestException exception)
            {
                switch (exception.StatusCode)
                {
                    case 404:
                        return NotFound(exception.Message);
                        break;
                    case 400:
                        return NotFound(exception.Message);
                        break;
                    default: throw;
                }


            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiKeyResponse>>> GetAllKeys(string username, string password)
        {

            try
            {
                var user = await _apikeyService.GetAllApiKey(username, password);
           
                return Ok(user.Select(apikey => apikey.MapToApiKeyResponse()));
            }
            catch (BadHttpRequestException exception)
            {
                switch (exception.StatusCode)
                {
                    case 404:
                        return NotFound(exception.Message);
                        break;
                    case 400:
                        return NotFound(exception.Message);
                        break;
                    default: throw;
                }


            }
        }

        [HttpPut]
        [Route("{id}/isActive")]
        public async Task<ActionResult<ApiKeyResponse>> UptadeKeyState(Guid id, UptateStateRequest request)
        {
            var apiKey = await _apikeyService.UpdateApiKey(id, request.IsActive);


            return apiKey.MapToApiKeyResponse();
        }
    }
}