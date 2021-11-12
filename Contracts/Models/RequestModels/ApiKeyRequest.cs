using System;

namespace Contracts.Models.RequestModels
{
    public class ApiKeyRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}