using EventManagementSystem.API.DTOs;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;


namespace EventManagementSystem.API.Controllers.Organizer
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IGenericRepository<Event> _genericRepository;
        public EventController(IGenericRepository<Event> genericRepository)
        {
            _genericRepository = genericRepository;
        }


        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Event _event)
        {
            if (_event == null)
                return BadRequest("Assesmetn is null");

            try
            {
                await _genericRepository.AddAsync(_event);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occured while saving the assesment");
            }
            return Ok(_event);
        }

    }
}
