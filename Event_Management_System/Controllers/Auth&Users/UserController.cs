using System.Text.Json;
using EventManagementSystem.Core.DTOs;
using EventManagementSystem.API.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == null || id < 0) return BadRequest("Id format is not valid");

            var user = await _repository.GetByIdAsync(id);
            if (user == null) return NotFound("User not found");
            var userDto = new RegisterUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
            };
            return Ok(userDto);
        }

        [HttpGet]
        public  IActionResult GetAll()
        {
            var users =  _repository.GetAll();
            if (users == null) return NotFound("no users");
            return Ok(users);
        }


    }
}
