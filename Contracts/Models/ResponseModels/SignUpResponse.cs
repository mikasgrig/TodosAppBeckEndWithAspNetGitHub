using System;

namespace Contracts.Models.ResponseModels
{
    public class SignUpResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateCreDate { get; set; }
    }
}