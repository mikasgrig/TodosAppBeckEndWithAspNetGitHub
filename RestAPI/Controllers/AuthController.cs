using System;
using System.Threading.Tasks;
using Contracts.Models.RequestModels.UserModels;
using Contracts.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Persistence.Models.ReadModels;
using Persistence.Repositories;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost]
        [Route("singUp")]
        public async Task<ActionResult<SignUpResponse>> SingUp(SignUpRequest request)
        {
            var currentUser = await _userRepository.GetAsync(request.UserName);
            if (currentUser is not null)
            {
                return BadRequest($"user with Username: {request.UserName} already exists!");
            }

            var userReadModel = new UserReadModel
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Password = request.Password,
                DateCreated = DateTime.Now
            };

            await _userRepository.SaveUserAsync(userReadModel);
            
            return Ok(new SignUpResponse
            {
                Id = userReadModel.Id,
                UserName = userReadModel.UserName,
                DateCreDate = userReadModel.DateCreated
            });
        }
    }
}