using EventManagementSystem.API.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.API.Controllers.Auth_Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // view client Profile and Manage (Client)

        private readonly IGenericRepository<User> _repository;
        public UserController(IGenericRepository<User> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            var userDto = new RegisterUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
            };
            return Ok(userDto);
        }

    }
}
