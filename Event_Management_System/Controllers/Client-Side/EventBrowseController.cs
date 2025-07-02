using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.API.Controllers.Client_Side
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventBrowseController : ControllerBase
    {
        private readonly IGenericRepository<Event> _genericRepository;

        public EventBrowseController(IGenericRepository<Event> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        // Search, Filter, View events

        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _genericRepository.GetAll().ToListAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the events: " + ex.Message);
            }
        }

        [HttpGet("GetEventById/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventItem = await _genericRepository.GetByIdAsync(id);
                if (eventItem == null)
                {
                    return NotFound("Event not found");
                }
                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the event: " + ex.Message);
            }
        }

        [HttpGet("SearchEvents/{searchTerm}")]
        public async Task<IActionResult> SearchEvents(string searchTerm)
        {
            try
            {
                var events = await _genericRepository
                    .FindByCondition(e => e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm))
                    .ToListAsync();

                if (!events.Any())
                {
                    return NotFound("No events found matching the search term");
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while searching for events: " + ex.Message);
            }
        }

        [HttpGet("FilterEventsByCategory/{categoryId}")]
        public async Task<IActionResult> FilterEventsByCategory(int categoryId)
        {
            try
            {
                var events = await _genericRepository
                    .FindByCondition(e => e.CategoryId == categoryId)
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while filtering events: " + ex.Message);
            }
        
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
        {
            var events = _genericRepository
                .GetPaged(e => string.IsNullOrEmpty(keyword) || e.Title.Contains(keyword), page, pageSize);

            return Ok(await events.ToListAsync());
        }


    }
}
