using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Models.RequestModels.UserModels
{
    public class SignUpRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLengthAttribute(8)]
        public string Password { get; set; }
    }
}